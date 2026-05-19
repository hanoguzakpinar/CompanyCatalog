using CompanyCatalog.Application.Abstractions.Results;
using FluentValidation;
using MediatR;

namespace CompanyCatalog.Application.Abstractions.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var firstError = failures[0];
        var error = Error.Validation(firstError.PropertyName, firstError.ErrorMessage);

        var resultType = typeof(TResponse);
        if (resultType.IsGenericType)
        {
            var valueType = resultType.GetGenericArguments()[0];
            var method = typeof(Result)
                .GetMethods()
                .First(m => m.Name == "Failure" && m.IsGenericMethod)
                .MakeGenericMethod(valueType);
            return (TResponse)method.Invoke(null, new object[] { error })!;
        }

        return (TResponse)(object)Result.Failure(error);
    }
}