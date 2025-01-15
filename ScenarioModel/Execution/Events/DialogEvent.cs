using Newtonsoft.Json;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.Execution.Events;

[NodeLike<IScenarioNode, IfNode>]
public record DialogEvent : IScenarioEvent<DialogNode>
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
