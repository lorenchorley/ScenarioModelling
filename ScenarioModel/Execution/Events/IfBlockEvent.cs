using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Objects.ScenarioNodes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScenarioModel.Execution.Events;

public record IfBlockEvent : IScenarioEvent<IfNode>
{
    public bool IfBlockRun { get; set; }

    [JsonIgnore]
    public IfNode ProducerNode { get; init; } = null!;

    public override string ToString()
    {
        string objText = JsonSerializer.Serialize(this);
        return $"{GetType().Name} {objText}";
    }
}
