using ScenarioModel.Objects.SystemObjects.Interfaces;

namespace ScenarioModel.Collections.Graph;

public interface IDirectedGraphNode<T> : IIdentifiable where T : IDirectedGraphNode<T>
{
    /// <summary>
    /// Allows for iterating over all associated subgraphs possible from this node
    /// </summary>
    /// <returns></returns>
    IEnumerable<SemiLinearSubGraph<T>> TargetSubgraphs();
}
