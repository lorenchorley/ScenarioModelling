using ScenarioModelling.Mocks.Utils;

namespace ScenarioModelling.Mocks;

public class MetaStoryMock
{
    private readonly IServiceProvider _serviceProvider;
    internal string TargetMetaStoryName { get; set; }

    internal BusinessToStateMap BusinessObjectToStateMap { get; set; }

    public MetaStoryMock(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public MockCallBuilder CreateCallBuilder()
    {
        return new MockCallBuilder(_serviceProvider)
        {
            TargetMetaStoryName = TargetMetaStoryName,
            BusinessObjectToStateMap = BusinessObjectToStateMap
        };
    }
}
