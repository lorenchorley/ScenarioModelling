using Microsoft.Extensions.DependencyInjection;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Mocks;

namespace ScenarioModelling;

public class ScenarioModellingContainerScope : IDisposable
{
    private readonly IServiceScope _scope;

    public Context Context { get; private set; } = null!;

    public ScenarioModellingContainerScope(IServiceScope scope)
    {
        _scope = scope;

        SetExposedProperties();
    }

    private void SetExposedProperties()
    {
        Context = GetService<Context>();
    }

    public MetaStoryMockBuilder CreateMetaStoryMockBuilder()
    {
        MetaStoryMockBuilder metaStoryMockBuilder = GetService<MetaStoryMockBuilder>();
        return metaStoryMockBuilder;
    }

    public T GetService<T>() where T : notnull
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

}
