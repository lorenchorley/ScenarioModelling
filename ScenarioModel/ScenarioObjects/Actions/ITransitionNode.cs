namespace ScenarioModel.SystemObjects.Entities;

public interface ITransitionNode : IScenarioNode
{
    IEnumerable<string> TargetNodeNames { get; }
}
