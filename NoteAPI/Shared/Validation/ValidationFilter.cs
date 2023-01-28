using FluentValidation;

namespace NoteAPI.Shared.Validation;

public class ValidationFilter<T> : IEndpointFilter
    where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.Single(x => x!.GetType() == typeof(T)) as T;

        var validationResult = await _validator.ValidateAsync(argument!);
        
        return validationResult.IsValid
            ? await next(context)
            : Results.BadRequest(validationResult);
    }
}