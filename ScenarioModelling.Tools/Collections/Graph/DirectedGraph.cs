using ProtoBuf;
using ScenarioModelling.Tools.Collections.Graph.Validation;

namespace ScenarioModelling.Tools.Collections.Graph;

[ProtoContract]
public class DirectedGraph<T> where T : IDirectedGraphNode<T>
{
    [ProtoMember(1)]
    public ISubGraph<T> PrimarySubGraph { get; private set; }

    public DirectedGraph(ISubGraph<T> primarySubGraph)
    {
        PrimarySubGraph = primarySubGraph;
    }

    public T? Find(Func<T, bool> predicate)
    {
        return FindOnSubgraph(PrimarySubGraph, predicate);
    }

    private T? FindOnSubgraph(ISubGraph<T> subgraph, Func<T, bool> predicate)
    {
        T? result = subgraph.UnorderedEnumerable.FirstOrDefault(predicate);

        if (result != null)
        {
            return result;
        }

        foreach (var node in subgraph.UnorderedEnumerable)
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

        foreach (var node in PrimarySubGraph.UnorderedEnumerable)
        {
            if (node is not ITransitionNode transitionNode)
            {
                continue;
            }

            foreach (var targetNode in transitionNode.TargetNodeNames)
            {
                if (!PrimarySubGraph.UnorderedEnumerable.Any(n => n.Name.IsEqv(targetNode)))
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

    ///// <summary>
    ///// Reinitialise all subgraphs
    ///// </summary>
    //public void Reinitalise()
    //{
    //    HashSet<SemiLinearSubGraph<T>> visited = new();
    //    RecursivelyReinitialise(PrimarySubGraph, visited);
    //}

    //private void RecursivelyReinitialise(SemiLinearSubGraph<T> subgraph, HashSet<SemiLinearSubGraph<T>> visited)
    //{
    //    subgraph.Reinitalise();
    //    visited.Add(subgraph);

    //    foreach (var subsubgraph in subgraph.NodeSequence.SelectMany(node => node.TargetSubgraphs()))
    //    {
    //        if (visited.Contains(subsubgraph))
    //        {
    //            continue;
    //        }

    //        RecursivelyReinitialise(subsubgraph, visited);
    //    }
    //}

    //public DirectedGraph<T> CopyGraph()
    //{
    //    var newGraph = new DirectedGraph<T>();

    //    newGraph.PrimarySubGraph = RecursivelyCopy(PrimarySubGraph);

    //    return newGraph;
    //}

    //private SemiLinearSubGraph<T> RecursivelyCopy(SemiLinearSubGraph<T> subgraph)
    //{
    //    SemiLinearSubGraph<T> copy = new();

    //    foreach (T node in subgraph.NodeSequence)
    //    {
    //        copy.NodeSequence.Add(node);

    //        var targetSubgraphs = node.TargetSubgraphs();

    //        RecursivelyCopy(subsubgraph);
    //    }

    //    foreach (var subsubgraph in )
    //    {
            
    //    }
    //}
}
