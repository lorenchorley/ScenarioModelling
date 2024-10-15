using ScenarioModel.ScenarioObjects;
using ScenarioModel.SystemObjects.States;
using System.Collections;

namespace ScenarioModel.Collections;

public interface IDirectedGraphNode : INameful
{
}

public class DirectedGraph<T> : IEnumerable<T> where T : IDirectedGraphNode
{
    private readonly List<T> _nodes = new();

    public void Add(T node)
    {
        _nodes.Add(node);
    }
    
    public void AddRange(IEnumerable<T> nodes)
    {
        _nodes.AddRange(nodes);
    }

    public T? GetNextInSequence(T node)
    {
        var index = _nodes.IndexOf(node);

        if (index == -1 || index == _nodes.Count - 1)
        {
            return default;
        }

        return _nodes[index + 1];
    }

    public DirectedGraphValidationResult Validate()
    {
        var brokenLinks = new List<(IDirectedGraphNode node, string intended)>();

        foreach (var node in _nodes)
        {
            if (node is not ITransitionNode transitionNode)
            {
                continue;
            }

            foreach (var targetNode in transitionNode.TargetNodeNames)
            {
                if (!_nodes.Any(n => n.Name.IsEqv(targetNode)))
                {
                    brokenLinks.Add((node, targetNode));
                }
            }
        }

        if (brokenLinks.Count > 0)
        {
            return DirectedGraphValidationResult.BrokenLinks(brokenLinks);
        }
        else
        {
            return DirectedGraphValidationResult.Valid();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _nodes.GetEnumerator();
    }
}

public interface DirectedGraphValidationResult
{
    public static DirectedGraphValidationResult BrokenLinks(List<(IDirectedGraphNode node, string intendedTarget)> value) => new BrokenLinks(value);
    public static DirectedGraphValidationResult Valid() => new Valid();
}

public class BrokenLinks(List<(IDirectedGraphNode node, string intendedTarget)> links) : DirectedGraphValidationResult
{
    public List<(IDirectedGraphNode node, string intendedTarget)> Links { get; } = links;
}

public class Valid : DirectedGraphValidationResult
{
}
