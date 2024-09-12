using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

public class StateTransitionAction : IScenarioAction
{
    public string Name { get; set; } = "";
    public IStateful? StatefulObject { get; set; }
    public string StateName { get; set; } = "";

    public IEnumerable<string> TargetNodeNames => throw new NotImplementedException();
}
