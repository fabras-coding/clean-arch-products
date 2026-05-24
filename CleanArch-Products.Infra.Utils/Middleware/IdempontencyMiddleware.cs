using System.Text;
using CleanArch_Products.Application.Idempotency;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CleanArch_Products.Infra.Utils.Middleware
{
    /// <summary>
    /// Implements idempotency via the "Idempotency-Key" request header.
    /// Only applies to state-mutating methods: POST, PUT, PATCH.
    ///
    /// Flow:
    ///   1. No header → pass through (idempotency not requested)
    ///   2. Key exists + COMPLETED → replay cached response (HTTP 200 + X-Idempotent-Replayed: true)
    ///   3. Key exists + PROCESSING → another instance is handling it → HTTP 409 + Retry-After
    ///   4. Key not found → reserve atomically → execute → save response
    ///   5. Execution throws → delete reservation (client can retry with same key)
    /// </summary>
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IdempotencyMiddleware> _logger;
        private static readonly TimeSpan DefaultTtl = TimeSpan.FromHours(24);
        private const string IdempotencyHeader = "Idempotency-Key";

        public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IIdempotencyStore store)
        {
            // Only guard mutating methods
            if (!IsMutatingMethod(context.Request.Method))
            {
                await _next(context);
                return;
            }

            // If no Idempotency-Key header, just pass through (don't enforce — you could return 400 here instead)
            if (!context.Request.Headers.TryGetValue(IdempotencyHeader, out var keyValues) ||
                string.IsNullOrWhiteSpace(keyValues.ToString()))
            {
                await _next(context);
                return;
            }

            var rawKey = keyValues.ToString().Trim();

            // Validate it's a GUID to prevent key injection
            if (!Guid.TryParse(rawKey, out _))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"Idempotency-Key must be a valid UUID.\"}");
                return;
            }

            var storeKey = $"idempotency:{rawKey}";

            // --- STEP 1: Check for existing response ---
            var existing = await store.GetResponseAsync(storeKey);
            if (existing.HasValue)
            {
                _logger.LogInformation("Idempotency replay for key {Key}", rawKey);
                context.Response.StatusCode = existing.Value.StatusCode;
                context.Response.ContentType = "application/json";
                context.Response.Headers["X-Idempotent-Replayed"] = "true";
                await context.Response.WriteAsync(existing.Value.Body);
                return;
            }

            // --- STEP 2: Try to win the atomic reservation ---

            try
            {


                var reserved = await store.TryReserveAsync(storeKey, DefaultTtl);
                if (!reserved)
                {
                    // Key exists but response is null → still PROCESSING (concurrent duplicate request)
                    _logger.LogWarning("Idempotency conflict: key {Key} is still processing", rawKey);
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    context.Response.ContentType = "application/json";
                    context.Response.Headers["Retry-After"] = "1";
                    await context.Response.WriteAsync(
                        "{\"message\":\"A request with this Idempotency-Key is already being processed. Retry after 1 second.\"}");
                    return;
                }
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogError(ex, "Redis unavailable");
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    "{\"message\":\"Service is temporarily unavailable. Please try again later.\"}");
                return;
            }

            // --- STEP 3: We won the reservation — intercept the response body ---
            var originalBody = context.Response.Body;
            using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            try
            {
                await _next(context);

                // Capture what was written
                buffer.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(buffer, Encoding.UTF8).ReadToEndAsync();

                // Save only successful or client-error responses (not 5xx — those are transient)
                if (context.Response.StatusCode < 500)
                {
                    await store.SaveResponseAsync(storeKey, context.Response.StatusCode, body, DefaultTtl);
                    _logger.LogInformation("Idempotency response saved for key {Key} → HTTP {Status}",
                        rawKey, context.Response.StatusCode);
                }
                else
                {
                    // Server error: delete reservation so client can retry with same key
                    await store.DeleteReservationAsync(storeKey);
                    _logger.LogWarning("Idempotency reservation deleted (5xx) for key {Key}", rawKey);
                }

                // Write the buffered response to the real output
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBody);
            }
            catch (Exception ex)
            {
                // On unhandled exception: release the reservation
                await store.DeleteReservationAsync(storeKey);
                _logger.LogError(ex, "Exception during idempotent request — reservation released for key {Key}", rawKey);
                context.Response.Body = originalBody;
                throw;
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        private static bool IsMutatingMethod(string method) =>
            HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsPatch(method);
    }
}