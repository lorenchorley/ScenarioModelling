using Newtonsoft.Json;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.Execution.Events;

[StoryNodeLike<IStoryNode, IfNode>]
public record JumpEvent : IMetaStoryEvent<JumpNode>
{
    public string Target { get; set; } = "";

    [JsonIgnore]
    public JumpNode ProducerNode { get; set; } = null!;

    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
