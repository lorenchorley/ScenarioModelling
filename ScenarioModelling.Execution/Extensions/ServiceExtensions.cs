using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScenarioModelling.Execution.Dialog;

namespace ScenarioModelling.Execution.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<EventGenerationDependencies>();
        services.AddTransient<IExecutor, DialogExecutor>();
        services.AddTransient<DialogExecutor>();
        services.AddTransient<IExecutor, ParallelConstructionExecutor>();
        services.AddTransient<ParallelConstructionExecutor>();
        services.AddTransient<Story>();
    }
}
