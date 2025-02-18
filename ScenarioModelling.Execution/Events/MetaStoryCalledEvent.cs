using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Events;

[StoryNodeLike<IMetaStoryEvent, CallMetaStoryNode>]
public record MetaStoryCalledEvent : IMetaStoryEvent<CallMetaStoryNode>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public CallMetaStoryNode ProducerNode { get; set; } = null!;

    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
