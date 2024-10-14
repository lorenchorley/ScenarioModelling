using ScenarioModel.Execution.Events;
using ScenarioModel.References;
using ScenarioModel.ScenarioObjects.Events;

namespace ScenarioModel.ScenarioObjects;

public class StateTransitionNode : ITransitionNode
{
    public string Name { get; set; } = "";
    public IStatefulObjectReference? StatefulObject { get; set; }
    public string StateName { get; set; } = "";

    public IEnumerable<string> TargetNodeNames => [StateName];

    public IScenarioEvent ProduceEvent(string choice)
    {
        return new StateChangeEvent { };
    }
}
