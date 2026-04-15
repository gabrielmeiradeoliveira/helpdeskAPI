using System.Diagnostics;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // 1. tenta pegar do header
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();

        // 2. se não existir, cria
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // 3. coloca no contexto (pra usar depois)
        context.Items["CorrelationId"] = correlationId;

        // 4. devolve no response
        context.Response.Headers[HeaderName] = correlationId;

        // 5. adiciona no scope de log (ESSENCIAL)
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            _logger.LogInformation("Request started {Method} {Path}", 
                context.Request.Method, 
                context.Request.Path);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation(
                    "Request finished {StatusCode} in {ElapsedMs}ms",
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds
                );
            }
        }
    }
}