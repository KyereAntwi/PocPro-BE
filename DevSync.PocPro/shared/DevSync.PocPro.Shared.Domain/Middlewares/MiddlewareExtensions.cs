using Microsoft.AspNetCore.Builder;

namespace DevSync.PocPro.Shared.Domain.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}