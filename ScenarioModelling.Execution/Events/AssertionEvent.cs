using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Events;

[StoryNodeLike<IMetaStoryEvent, AssertNode>]
public record AssertionEvent : IMetaStoryEvent<AssertNode>
{
    public string Expression { get; set; } = ""; // Progressive series of evaluations to improve readability
    public bool AssertionSucceeded { get; set; }

    [JsonIgnore]
    public AssertNode ProducerNode { get; set; } = null!;

    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
