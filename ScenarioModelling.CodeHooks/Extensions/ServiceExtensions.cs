﻿namespace ScenarioModelling.CodeHooks.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<MetaStoryHookOrchestratorForConstruction>();
    }
}
