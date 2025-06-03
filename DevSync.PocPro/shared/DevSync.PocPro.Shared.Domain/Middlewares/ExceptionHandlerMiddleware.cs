using System.Net;
using System.Text.Json;
using DevSync.PocPro.Shared.Domain.Dtos;
using Microsoft.AspNetCore.Http;

namespace DevSync.PocPro.Shared.Domain.Middlewares;

public class ExceptionHandlerMiddleware
{
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ConvertException(context, ex);
            }
        }

        private Task ConvertException(HttpContext context, Exception exception)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var result = new BaseResponse<object>("Operation failed", false)
            {
                Success = false,
                Errors = new List<string>()
            };

            switch (exception)
            {
                case BadRequestException badRequestException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result.Message = "Failed";
                    result.Errors = [badRequestException.Message];
                    break;
                case not null:
                    httpStatusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.StatusCode = (int)httpStatusCode;

            if (result.Message != string.Empty && result.Data != null)
                return context.Response.WriteAsync(JsonSerializer.Serialize(result));
            var response = new BaseResponse<object>("Operation failed", false)
            {
                Success = false,
                Message = result?.Message!,
                Errors = result?.Errors!
            };
                
            result = response;

            return context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }
    
}