using ScenarioModel.Collections.Graph.Validation;
using ScenarioModel.Objects.ScenarioNodes.Interfaces;

namespace ScenarioModel.Collections.Graph;

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
