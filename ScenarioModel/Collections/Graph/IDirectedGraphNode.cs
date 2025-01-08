using ScenarioModel.Objects.SystemObjects.Interfaces;

namespace ScenarioModel.Collections.Graph;

public interface IDirectedGraphNode<T> : IIdentifiable where T : IDirectedGraphNode<T>
{
    IEnumerable<SemiLinearSubGraph<T>> TargetSubgraphs();
}
