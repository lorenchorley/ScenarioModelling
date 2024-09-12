using System.Collections;

namespace ScenarioModel.Collections;

public interface IDirectedGraphNode
{
    string Name { get; }
    IEnumerable<string> TargetNodeNames { get; }
}

public class DirectedGraph<T> : IEnumerable<T> where T : IDirectedGraphNode
{
    private readonly List<T> _nodes = new();

    public void Add(T node)
    {
        _nodes.Add(node);
    }

    public DirectedGraphValidationResult Validate()
    {
        var brokenLinks = new List<(IDirectedGraphNode node, string intended)>();

        foreach (var node in _nodes)
        {
            foreach (var targetNode in node.TargetNodeNames)
            {
                if (!_nodes.Any(n => string.Equals(n.Name, targetNode)))
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
