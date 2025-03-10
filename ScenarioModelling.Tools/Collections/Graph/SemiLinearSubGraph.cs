using ProtoBuf;

namespace ScenarioModelling.Tools.Collections.Graph;

public interface ISubGraph<T> where T : IDirectedGraphNode<T>
{
    IEnumerable<T> UnorderedEnumerable { get; } 

    ISubGraphScope<T> GenerateScope(ISubGraphScope<T>? parentScope);

    bool Contains(T node);

    // TODO Add new methods so that NodeSequence doesn't need to be accessed directly
}

[ProtoContract]
public class SemiLinearSubGraph<T> : ISubGraph<T> where T : IDirectedGraphNode<T>
{
    [ProtoMember(1)]
    public List<T> NodeSequence { get; private set; } = [];

    public IEnumerable<T> UnorderedEnumerable => NodeSequence;

    public bool Contains(T node)
    {
        return NodeSequence.Contains(node);
    }

    public ISubGraphScope<T> GenerateScope(ISubGraphScope<T>? parentScope)
    {
        return new SemiLinearSubGraphScope<T>(this, parentScope);
    }

    public void AddToSequence(T newNode)
    {
        NodeSequence.Add(newNode);
    }

    public void AddRangeToSequence(IEnumerable<T> list)
    {
        NodeSequence.AddRange(list);
    }
}
