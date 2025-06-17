using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace StudentPortal.Middleware
{
    public class SimpleLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public SimpleLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine($"[Middleware] {DateTime.Now}: Request for {context.Request.Path}");
            await _next(context);
        }
    }

    // Extension method to make adding the middleware cleaner
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseSimpleLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SimpleLoggingMiddleware>();
        }
    }
}
