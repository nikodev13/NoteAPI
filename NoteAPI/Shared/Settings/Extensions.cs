namespace NoteAPI.Shared.Settings;

public static class Extensions
{
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
        where TOptions : ISettings, new()
    {
        var options = new TOptions();
        configuration.GetRequiredSection(TOptions.SectionName).Bind(options);
        return options;
    }

    public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration) 
        where TOptions : class, ISettings
    {
        services.AddOptions<TOptions>()
            .Bind(configuration.GetRequiredSection(TOptions.SectionName));
    }
}