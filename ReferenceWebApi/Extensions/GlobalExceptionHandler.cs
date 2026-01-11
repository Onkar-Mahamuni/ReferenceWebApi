using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ReferenceWebApi.Application.Exceptions;

namespace ReferenceWebApi.Api.Configuration
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<GlobalExceptionHandler> logger)
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            // 1. Determine Status Code and Standard Details
            var (statusCode, title, detail) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found", exception.Message),
                ValidationException => (StatusCodes.Status400BadRequest, "Validation Failed", exception.Message),
                BusinessException => (StatusCodes.Status400BadRequest, "Business Rule Violation", exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred")
            };

            // 2. Construct the ProblemDetails object
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Type = exception.GetType().Name,
                Instance = httpContext.Request.Path
            };

            // 3. Handle specific extensions (like Validation Errors)
            if (exception is ValidationException validationEx)
            {
                problemDetails.Extensions["errors"] = validationEx.Errors;
            }

            // 4. Add TraceId (Standard practice)
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = statusCode;

            // 5. Use the built-in service to write the response
            // This respects content negotiation and standardizes the JSON format
            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
        }
    }
}



//using Microsoft.AspNetCore.Diagnostics;
//using ReferenceWebApi.Application.Exceptions;
//using System.Net;

//namespace ReferenceWebApi.Api.Extensions
//{
//    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
//    {
//        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

//        public async ValueTask<bool> TryHandleAsync(
//            HttpContext httpContext,
//            Exception exception,
//            CancellationToken cancellationToken)
//        {
//            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

//            var (statusCode, response) = exception switch
//            {
//                NotFoundException notFound => (
//                    HttpStatusCode.NotFound,
//                    new ErrorResponse
//                    {
//                        Title = "Resource Not Found",
//                        Status = (int)HttpStatusCode.NotFound,
//                        Detail = notFound.Message,
//                        TraceId = httpContext.TraceIdentifier
//                    }),

//                ValidationException validation => (
//                    HttpStatusCode.BadRequest,
//                    new ValidationErrorResponse
//                    {
//                        Title = "Validation Failed",
//                        Status = (int)HttpStatusCode.BadRequest,
//                        Detail = validation.Message,
//                        TraceId = httpContext.TraceIdentifier,
//                        Errors = validation.Errors
//                    }),

//                BusinessException business => (
//                    HttpStatusCode.BadRequest,
//                    new ErrorResponse
//                    {
//                        Title = "Business Rule Violation",
//                        Status = (int)HttpStatusCode.BadRequest,
//                        Detail = business.Message,
//                        TraceId = httpContext.TraceIdentifier
//                    }),

//                _ => (
//                    HttpStatusCode.InternalServerError,
//                    new ErrorResponse
//                    {
//                        Title = "Internal Server Error",
//                        Status = (int)HttpStatusCode.InternalServerError,
//                        Detail = "An unexpected error occurred",
//                        TraceId = httpContext.TraceIdentifier
//                    })
//            };

//            httpContext.Response.StatusCode = (int)statusCode;

//            // WriteAsJsonAsync handles Content-Type and serialization options automatically
//            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

//            return true; // Signal that the exception was handled successfully
//        }

//        private class ErrorResponse
//        {
//            public string Title { get; set; } = string.Empty;
//            public int Status { get; set; }
//            public string Detail { get; set; } = string.Empty;
//            public string TraceId { get; set; } = string.Empty;
//        }

//        private class ValidationErrorResponse : ErrorResponse
//        {
//            public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
//        }
//    }
//}
