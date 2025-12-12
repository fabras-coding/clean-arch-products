using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace CleanArch_Products.Infra.Utils
{
    public class CorrelationIdMiddleware
    {
        
        private readonly RequestDelegate _next;
        private const string HeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            //check if the incoming request contains the correlation ID header
            var correlationId = context.Request.Headers.ContainsKey(HeaderName) ? context.Request.Headers[HeaderName].ToString() : Guid.NewGuid().ToString();

            //add the correlation ID to the response headers
            context.Response.Headers[HeaderName] = correlationId;
            
            //add the correlation to the context for further usage in logs
            context.Items[HeaderName] = correlationId;

            using(LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }

        }


    }
}