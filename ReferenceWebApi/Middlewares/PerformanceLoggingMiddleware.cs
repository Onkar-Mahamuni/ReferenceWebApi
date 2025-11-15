using System.Diagnostics;

namespace ReferenceWebApi.Api.Middlewares
{
    public class PerformanceLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private const int SlowRequestThresholdMs = 1000;

        public PerformanceLoggingMiddleware(
            RequestDelegate next,
            ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var request = context.Request;
                var response = context.Response;
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                var logLevel = elapsedMs > SlowRequestThresholdMs
                    ? LogLevel.Warning
                    : LogLevel.Information;

                _logger.Log(
                    logLevel,
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    request.Method,
                    request.Path,
                    response.StatusCode,
                    elapsedMs);

                if (elapsedMs > SlowRequestThresholdMs)
                {
                    _logger.LogWarning(
                        "Slow Request: {Method} {Path} took {ElapsedMs}ms",
                        request.Method,
                        request.Path,
                        elapsedMs);
                }
            }
        }
    }
}
