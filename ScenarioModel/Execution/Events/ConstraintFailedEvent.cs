using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Objects.SystemObjects;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScenarioModelling.Execution.Events;

public record ConstraintFailedEvent : IScenarioEvent
{
    public string Expression { get; set; } = ""; // Progressive series of evaluations to improve readability

    [JsonIgnore]
    public Constraint ProducerNode { get; init; } = null!;

    public override string ToString()
    {
        string objText = JsonSerializer.Serialize(this);
        return $"{GetType().Name} {objText}";
    }
}
