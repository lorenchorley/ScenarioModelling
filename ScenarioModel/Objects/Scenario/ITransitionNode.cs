namespace ScenarioModel.Objects.Scenario;

public interface ITransitionNode : IScenarioNode
{
    IEnumerable<string> TargetNodeNames { get; }
}
