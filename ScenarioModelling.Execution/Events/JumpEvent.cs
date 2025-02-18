using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Events;

[StoryNodeLike<IMetaStoryEvent, JumpNode>]
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
