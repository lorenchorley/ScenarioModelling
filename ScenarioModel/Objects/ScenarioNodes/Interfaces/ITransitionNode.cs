using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.Objects.ScenarioNodes.Interfaces;

public interface ITransitionNode : IScenarioNode
{
    IEnumerable<string> TargetNodeNames { get; }
}
