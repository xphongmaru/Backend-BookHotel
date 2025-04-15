using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using BookHotel.Responsee;
using BookHotel.Exceptions;

namespace BookHotel.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Xác định mã lỗi phù hợp
            int statusCode = exception switch
            {
                BadRequestException => (int)HttpStatusCode.BadRequest,         // 400
                NotFoundException => (int)HttpStatusCode.NotFound,             // 404
                _ => (int)HttpStatusCode.InternalServerError                   // 500
            };
            context.Response.StatusCode = statusCode;

            // Tạo BaseResponse trả về
            var errorResponse = new BaseResponse<string>(exception.Message, statusCode);

            var json = JsonSerializer.Serialize(errorResponse);
            return context.Response.WriteAsync(json);
        }
    }
}
