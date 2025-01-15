namespace ScenarioModelling.Collections.Graph.Validation;

public class BrokenLinksResult<T>(List<(IDirectedGraphNode<T> node, string intendedTarget)> links) : DirectedGraphValidationResult<T> where T : IDirectedGraphNode<T>
{
    public List<(IDirectedGraphNode<T> node, string intendedTarget)> Links { get; } = links;
}
