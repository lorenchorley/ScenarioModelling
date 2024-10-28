using ScenarioModel.Execution.Events;
using ScenarioModel.References;

namespace ScenarioModel.Objects.Scenario;

public record StateTransitionNode : IScenarioNode<StateChangeEvent>
{
    public string Name { get; set; } = "";
    public IStatefulObjectReference? StatefulObject { get; set; }
    public string TransitionName { get; set; } = "";

    public StateChangeEvent GenerateEvent()
    {
        ArgumentNullException.ThrowIfNull(StatefulObject);

        return new StateChangeEvent()
        {
            ProducerNode = this,
            StatefulObject = StatefulObject,
            TransitionName = TransitionName
        };
    }
}
