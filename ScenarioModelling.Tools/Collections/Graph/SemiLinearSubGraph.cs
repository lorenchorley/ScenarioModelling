using ProtoBuf;

namespace ScenarioModelling.Tools.Collections.Graph;

[ProtoContract]
public class SemiLinearSubGraph<T> where T : IDirectedGraphNode<T>
{
    [ProtoMember(1)]
    public List<T> NodeSequence { get; private set; } = [];

    public T FirstNode => NodeSequence.First();

    public SemiLinearSubGraphScope<T> GenerateScope(SemiLinearSubGraphScope<T>? parentScope)
    {
        return new(this, parentScope);
    }
}
