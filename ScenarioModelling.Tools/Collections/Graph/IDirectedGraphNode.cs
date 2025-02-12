using ScenarioModelling.Tools.GenericInterfaces;

namespace ScenarioModelling.Tools.Collections.Graph;

public interface IDirectedGraphNode<T> : INamed where T : IDirectedGraphNode<T>
{
    /// <summary>
    /// Allows for iterating over all associated subgraphs possible from this node
    /// </summary>
    /// <returns></returns>
    IEnumerable<SemiLinearSubGraph<T>> TargetSubgraphs();
}
