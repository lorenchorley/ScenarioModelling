using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScenarioModelling.Execution.Dialog;

namespace ScenarioModelling.Execution.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.TryAddSingleton<EventGenerationDependencies>();
        services.TryAddSingleton<IExecutor, DialogExecutor>();
        services.TryAddSingleton<DialogExecutor>();
    }
}
