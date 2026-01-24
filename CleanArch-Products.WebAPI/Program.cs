using CleanArch_Products.Infra.IoC;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Serilog;
using CleanArch_Products.Infra.Utils;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureAPI(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // ajuste se necessário
            .AllowAnyHeader()
            .AllowAnyMethod();
            // .AllowCredentials(); // só se precisar de cookies
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // evita o comportamento padrão
                context.HandleResponse();

                // Garantir CORS header como fallback (apenas se necessário)
                // Se UseCors estiver corretamente aplicado, isso não será necessário,
                // mas adicionamos para garantir que o navegador não bloqueie a resposta.
                context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var payload = JsonSerializer.Serialize(new { message = "Unauthorized" });
                return context.Response.WriteAsync(payload);
            },

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception?.Message);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Serilog (mantém seu setup)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.DatadogLogs(
        apiKey: builder.Configuration["Datadog:ApiKey"],
        source: "products-webapi",
        service: "products-webapi",
        host: Environment.MachineName,
        tags: new[] { "env:development", "project:clean-arch-products" }
    ).CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Ordem do pipeline IMPORTA
app.UseCors("AllowReactApp");

// middleware custom (correlation id) pode ficar depois do CORS
app.UseMiddleware<CorrelationIdMiddleware>();

// Autenticação e autorização devem estar no pipeline
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
