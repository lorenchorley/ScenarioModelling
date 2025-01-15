using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.References;
using ScenarioModelling.References.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScenarioModelling.Execution.Events;

[NodeLike<IScenarioNode, IfNode>]
public record StateChangeEvent : IScenarioEvent<TransitionNode>
{
    public IReference StatefulObject { get; set; } = null!;
    public StateReference InitialState { get; set; } = null!;
    public StateReference FinalState { get; set; } = null!;
    public string TransitionName { get; set; } = null!;

    [JsonIgnore]
    public TransitionNode ProducerNode { get; init; } = null!;

    public override string ToString()
    {
        string objText = JsonSerializer.Serialize(this);
        return $"{GetType().Name} {objText}";
    }
}
