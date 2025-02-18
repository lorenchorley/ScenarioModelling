using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Events;

[StoryNodeLike<IMetaStoryEvent, DialogNode>]
public record DialogEvent : IMetaStoryEvent<DialogNode>
{
    public string Text { get; set; } = null!;
    public string? Character { get; set; } = null;

    [JsonIgnore]
    public DialogNode ProducerNode { get; set; } = null!;

    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
