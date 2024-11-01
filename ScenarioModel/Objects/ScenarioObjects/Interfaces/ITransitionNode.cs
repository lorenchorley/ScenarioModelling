using ScenarioModel.Objects.ScenarioObjects.BaseClasses;

namespace ScenarioModel.Objects.ScenarioObjects.Interfaces;

public interface ITransitionNode : IScenarioNode
{
    IEnumerable<string> TargetNodeNames { get; }
}
