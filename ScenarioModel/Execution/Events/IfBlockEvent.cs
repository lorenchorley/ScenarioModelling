using Newtonsoft.Json;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.Execution.Events;

[StoryNodeLike<IStoryNode, IfNode>]
public record IfBlockEvent : IMetaStoryEvent<IfNode>
{
    public string Expression { get; set; } = ""; // Progressive series of evaluations to improve readability
    public bool IfBlockRun { get; set; }

    [JsonIgnore]
    public IfNode ProducerNode { get; set; } = null!;

    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
