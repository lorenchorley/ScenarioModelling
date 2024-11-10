using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.Objects.ScenarioNodes.Interfaces;

public interface ITransitionNode : IScenarioNode
{
    IEnumerable<string> TargetNodeNames { get; }
}
