using ReferenceWebApi.Api.Middlewares;

namespace ReferenceWebApi.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseResponseWrapper(
            this IApplicationBuilder app)
        {
            // This is the method that actually inserts the middleware into the pipeline.
            return app.UseMiddleware<ResponseWrapperMiddleware>();
        }

        //public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app)
        //{
        //}
    }
}
