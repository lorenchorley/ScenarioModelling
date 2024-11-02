using ScenarioModel.Objects.ScenarioObjects.Interfaces;
using ScenarioModel.Objects.SystemObjects;

namespace ScenarioModel.Collections;

public interface IDirectedGraphNode<T> : INameful where T : IDirectedGraphNode<T>
{
    IEnumerable<SemiLinearSubGraph<T>> TargetSubgraphs();
}

public class SemiLinearSubGraph<T> where T : IDirectedGraphNode<T>
{
    public List<T> NodeSequence { get; private set; } = new();
    public SemiLinearSubGraph<T>? ParentSubGraph { get; set; }
    public T? ParentSubGraphEntryPoint { get; set; }

    public T? GetNextInSequence(T node)
    {
        var index = NodeSequence.IndexOf(node); // Could be better

        if (index == -1 || index == NodeSequence.Count - 1)
        {
            return default;
        }

        return NodeSequence[index + 1];
    }

}

public class DirectedGraph<T> where T : IDirectedGraphNode<T>
{
    public SemiLinearSubGraph<T> PrimarySubGraph { get; private set; } = new();

    public void Add(T node)
    {
        PrimarySubGraph.NodeSequence.Add(node);
    }

    public void AddRange(IEnumerable<T> nodes)
    {
        PrimarySubGraph.NodeSequence.AddRange(nodes);
    }

    public T? Find(Func<T, bool> predicate)
    {
        return FindOnSubgraph(PrimarySubGraph, predicate);
    }

    private T? FindOnSubgraph(SemiLinearSubGraph<T> subgraph, Func<T, bool> predicate)
    {
        T? result = subgraph.NodeSequence.FirstOrDefault(predicate);

        if (result != null)
        {
            return result;
        }

        foreach (var node in subgraph.NodeSequence)
        {
            foreach (var targetSubgraph in node.TargetSubgraphs())
            {
                result = FindOnSubgraph(targetSubgraph, predicate);

                if (result != null)
                {
                    return result;
                }
            }
        }

        return default;
    }

    public DirectedGraphValidationResult<T> Validate()
    {
        var brokenLinks = new List<(IDirectedGraphNode<T> node, string intended)>();

        foreach (var node in PrimarySubGraph.NodeSequence)
        {
            if (node is not ITransitionNode transitionNode)
            {
                continue;
            }

            foreach (var targetNode in transitionNode.TargetNodeNames)
            {
                if (!PrimarySubGraph.NodeSequence.Any(n => n.Name.IsEqv(targetNode)))
                {
                    brokenLinks.Add((node, targetNode));
                }
            }
        }

        if (brokenLinks.Count > 0)
        {
            return DirectedGraphValidationResult<T>.BrokenLinks(brokenLinks);
        }
        else
        {
            return DirectedGraphValidationResult<T>.Valid();
        }
    }

}

public interface DirectedGraphValidationResult<T> where T : IDirectedGraphNode<T>
{
    public static DirectedGraphValidationResult<T> BrokenLinks(List<(IDirectedGraphNode<T> node, string intendedTarget)> value) => new BrokenLinks<T>(value);
    public static DirectedGraphValidationResult<T> Valid() => new Valid<T>();
}

public class BrokenLinks<T>(List<(IDirectedGraphNode<T> node, string intendedTarget)> links) : DirectedGraphValidationResult<T> where T : IDirectedGraphNode<T>
{
    public List<(IDirectedGraphNode<T> node, string intendedTarget)> Links { get; } = links;
}

public class Valid<T> : DirectedGraphValidationResult<T> where T : IDirectedGraphNode<T>
{
}
