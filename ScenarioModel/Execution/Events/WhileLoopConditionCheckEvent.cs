using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScenarioModelling.Execution.Events;

[NodeLike<IScenarioNode, IfNode>]
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
