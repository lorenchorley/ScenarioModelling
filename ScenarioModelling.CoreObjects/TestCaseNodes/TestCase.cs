using Newtonsoft.Json;
using ScenarioModelling.CoreObjects.TestCaseNodes.BaseClasses;

namespace ScenarioModelling.CoreObjects.TestCaseNodes;

/// <summary>
/// 
/// </summary>
public class TestCase : ITestCaseNode, IIdentifiable
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(MetaStory);

    public Context Context { get; private set; }

    public string MetaStoryName { get; private set; }

    public TestCase(Context context, string metaStoryName)
    {
        Context = context;
        MetaStoryName = metaStoryName;
    }
}
