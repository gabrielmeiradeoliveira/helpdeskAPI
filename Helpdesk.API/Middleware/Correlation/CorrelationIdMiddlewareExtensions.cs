public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(
        this IApplicationBuilder app,
        string headerName = "X-Correlation-ID")
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}