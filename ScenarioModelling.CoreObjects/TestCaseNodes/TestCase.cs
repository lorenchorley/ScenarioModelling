using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.CoreObjects.TestCaseNodes.BaseClasses;

namespace ScenarioModelling.CoreObjects.TestCaseNodes;

/// <summary>
/// 
/// </summary>
public class TestCase : ITestCaseNode, IIdentifiable
{
    [ProtoMember(1)]
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
