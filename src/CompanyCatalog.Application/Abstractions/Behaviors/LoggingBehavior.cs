using System.Diagnostics;
using CompanyCatalog.Application.Abstractions.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompanyCatalog.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("Handling {RequestName}", requestName);
            var stopwatch = Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();
            if (response.IsSuccess)
            {
                _logger.LogInformation(
                    "Completed {RequestName} in {Elapsed}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning(
                    "Completed {RequestName} with error {ErrorCode}: {ErrorMessage}",
                    requestName, response.Error.Code, response.Error.Message);
            }

            return response;
        }
    }
}