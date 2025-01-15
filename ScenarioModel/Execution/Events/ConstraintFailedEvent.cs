using Newtonsoft.Json;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.Execution.Events;

public record ConstraintFailedEvent : IScenarioEvent
{
    public string Expression { get; set; } = ""; // Progressive series of evaluations to improve readability

    [JsonIgnore]
    public Constraint ProducerNode { get; set; } = null!;

    public override string ToString()
    {
        string objText = JsonConvert.SerializeObject(this);
        return $"{GetType().Name} {objText}";
    }
}
