//using ProtoBuf;

namespace ScenarioModelling.Tools.Collections.Graph;

//[ProtoContract]
public class SemiLinearSubGraph<T> : ISubGraph<T> where T : IDirectedGraphNode<T>
{
    //[ProtoMember(1)]
    public List<T> NodeSequence { get; private set; } = [];

    public IEnumerable<T> UnorderedEnumerable => NodeSequence;

    public SemiLinearSubGraph()
    {
        
    }

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
