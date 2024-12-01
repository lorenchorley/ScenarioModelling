using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScenarioModel.Execution.Events;

[NodeLike<IScenarioNode, IfNode>]
public record JumpEvent : IScenarioEvent<JumpNode>
{
    public string Target { get; init; } = "";

    [JsonIgnore]
    public JumpNode ProducerNode { get; init; } = null!;

    public override string ToString()
    {
        string objText = JsonSerializer.Serialize(this);
        return $"{GetType().Name} {objText}";
    }
}
