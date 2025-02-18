using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Events;

[StoryNodeLike<IMetaStoryEvent, WhileNode>]
public record WhileLoopConditionCheckEvent : IMetaStoryEvent<WhileNode>
{
    public bool LoopBlockRun { get; set; }

    [JsonIgnore]
    public WhileNode ProducerNode { get; set; } = null!;

    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
