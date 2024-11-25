using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Objects.ScenarioNodes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScenarioModel.Execution.Events;

public record WhileLoopConditionCheckEvent : IScenarioEvent<WhileNode>
{
    public bool LoopBlockRun { get; set; }

    [JsonIgnore]
    public WhileNode ProducerNode { get; init; } = null!;

    public override string ToString()
    {
        string objText = JsonSerializer.Serialize(this);
        return $"{GetType().Name} {objText}";
    }
}
