namespace ScenarioModelling.Tools.Collections.Graph;

public interface ITransitionNode
{
    IEnumerable<string> TargetNodeNames { get; }
}
