namespace NoteAPI.Shared.Validation;

public static class Extensions
{
    public static RouteHandlerBuilder UseValidation<T>(this RouteHandlerBuilder endpoint)
        where T : class
    {
        endpoint.AddEndpointFilter<ValidationFilter<T>>();
        return endpoint;
    }
}