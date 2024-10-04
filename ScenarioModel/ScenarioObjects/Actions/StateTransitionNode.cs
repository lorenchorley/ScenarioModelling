using ScenarioModel.ScenarioObjects.Events;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

public class StateTransitionNode : IScenarioNode
{
    public string Name { get; set; } = "";
    public IStateful? StatefulObject { get; set; }
    public string StateName { get; set; } = "";

    public IEnumerable<string> TargetNodeNames => throw new NotImplementedException();
    
    public IScenarioEvent ProduceEvent(string choice)
    {
        return new StateChangeEvent { };
    }
}
