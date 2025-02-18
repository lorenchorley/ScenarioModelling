using Microsoft.Extensions.DependencyInjection;

namespace ScenarioModelling.TestDataAndTools;

public class TestContainer : ScenarioModellingContainer
{
    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<StoryTestRunner>();
    }
}
