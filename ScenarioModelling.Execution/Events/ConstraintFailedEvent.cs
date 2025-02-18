using Newtonsoft.Json;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Events;

public record ConstraintFailedEvent : IMetaStoryEvent
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
