using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScenarioModelling.Execution.Events;

[NodeLike<IScenarioNode, IfNode>]
public record ChoiceSelectedEvent : IScenarioEvent<ChooseNode>
{
    public string Choice { get; set; } = "";

    [JsonIgnore]
    public ChooseNode ProducerNode { get; init; } = null!;

    public override string ToString()
    {
        string objText = JsonSerializer.Serialize(this);
        return $"{GetType().Name} {objText}";
    }
}
