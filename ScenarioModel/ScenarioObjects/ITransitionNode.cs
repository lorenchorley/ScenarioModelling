namespace ScenarioModel.ScenarioObjects;

public interface ITransitionNode : IScenarioNode
{
    IEnumerable<string> TargetNodeNames { get; }
}
