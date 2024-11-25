using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScenarioModel.Execution.Events;

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
