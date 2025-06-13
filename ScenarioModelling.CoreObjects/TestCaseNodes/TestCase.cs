using Newtonsoft.Json;
using ScenarioModelling.CoreObjects.TestCaseNodes.Interfaces;

namespace ScenarioModelling.CoreObjects.TestCaseNodes;

/// <summary>
/// 
/// </summary>
public class TestCase : ITestCaseNode
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(MetaStory);

    public Context Context { get; private set; }

    public string MetaStoryName { get; private set; }

    public Dictionary<string, string> InitialStates { get; private set; } = new();
    public Dictionary<string, string> FinalStates { get; private set; } = new();

    public TestCase(Context context, string metaStoryName)
    {
        Context = context;
        MetaStoryName = metaStoryName;
    }
}
