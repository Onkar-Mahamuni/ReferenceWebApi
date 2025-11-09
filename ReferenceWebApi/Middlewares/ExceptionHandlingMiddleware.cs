using ReferenceWebApi.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace ReferenceWebApi.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var (statusCode, response) = exception switch
            {
                NotFoundException notFound => (
                    HttpStatusCode.NotFound,
                    new ErrorResponse
                    {
                        Title = "Resource Not Found",
                        Status = (int)HttpStatusCode.NotFound,
                        Detail = notFound.Message,
                        TraceId = context.TraceIdentifier
                    }),

                ValidationException validation => (
                    HttpStatusCode.BadRequest,
                    new ValidationErrorResponse
                    {
                        Title = "Validation Failed",
                        Status = (int)HttpStatusCode.BadRequest,
                        Detail = validation.Message,
                        TraceId = context.TraceIdentifier,
                        Errors = validation.Errors
                    }),

                BusinessException business => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Title = "Business Rule Violation",
                        Status = (int)HttpStatusCode.BadRequest,
                        Detail = business.Message,
                        TraceId = context.TraceIdentifier
                    }),

                _ => (
                    HttpStatusCode.InternalServerError,
                    new ErrorResponse
                    {
                        Title = "Internal Server Error",
                        Status = (int)HttpStatusCode.InternalServerError,
                        Detail = "An unexpected error occurred",
                        TraceId = context.TraceIdentifier
                    })
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

        private class ErrorResponse
        {
            public string Title { get; set; } = string.Empty;
            public int Status { get; set; }
            public string Detail { get; set; } = string.Empty;
            public string TraceId { get; set; } = string.Empty;
        }

        private class ValidationErrorResponse : ErrorResponse
        {
            public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
        }
    }
}
