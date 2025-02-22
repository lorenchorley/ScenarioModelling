namespace ScenarioModelling.Mocks.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<MetaStoryMockBuilder>();
        services.AddTransient<MockStoryRunner>();
        services.AddTransient<MockDialogExecutor>();
    }
}
