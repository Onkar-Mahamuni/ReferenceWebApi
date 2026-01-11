using ReferenceWebApi.Application.Dtos.Common;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ReferenceWebApi.Api.Middlewares
{
    public class ResponseWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                // Execute the pipeline
                await _next(context);

                // --- IF WE REACH HERE, THE PIPELINE COMPLETED WITHOUT UNHANDLED EXCEPTIONS ---

                // Restore the original stream immediately so we can write to it
                context.Response.Body = originalBodyStream;

                // Only wrap for successful (2xx) status codes.
                bool isSuccessful = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300;

                // Check content type to ensure we are wrapping JSON
                bool isJson = context.Response.ContentType?.Contains("application/json") == true;

                if (isJson && isSuccessful)
                {
                    // --- WRAP SUCCESSFUL RESPONSE ---
                    responseBody.Seek(0, SeekOrigin.Begin);
                    var text = await new StreamReader(responseBody).ReadToEndAsync();

                    var wrappedResponse = ApiResponse<JsonNode?>.SuccessResponse(
                        JsonSerializer.Deserialize<JsonNode?>(text),
                        "Request completed successfully");

                    wrappedResponse.TraceId = context.TraceIdentifier;

                    var wrappedText = JsonSerializer.Serialize(wrappedResponse);
                    var wrappedBytes = System.Text.Encoding.UTF8.GetBytes(wrappedText);

                    context.Response.ContentLength = wrappedBytes.Length;
                    await context.Response.Body.WriteAsync(wrappedBytes);
                }
                else
                {
                    // --- PASS-THROUGH LOGIC (Errors, non-JSON, etc) ---
                    // Just copy the contents of the memory stream to the original stream
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception)
            {
                // --- EXCEPTION HANDLING ---
                // If an exception occurs, we MUST restore the original body stream.
                // Otherwise, the upstream ExceptionHandler will try to write to the 
                // disposed MemoryStream ('responseBody'), causing a server crash.
                context.Response.Body = originalBodyStream;

                // Re-throw the exception so the Global Exception Handler can catch it 
                // and generate the correct ProblemDetails response.
                throw;
            }
        }
    }
}