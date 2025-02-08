using ProtoBuf;

namespace ScenarioModelling.Collections.Graph;

[ProtoContract]
public class SemiLinearSubGraph<T> where T : IDirectedGraphNode<T>
{
    [ProtoMember(1)]
    public List<T> NodeSequence { get; private set; } = [];
    public SemiLinearSubGraph<T>? ParentSubgraph { get; set; }

    private int _lastReturnedIndex = -1;
    private T? ExplicitNextNode { get; set; }

    public void SetExplicitNextNode(T node)
    {
        // Node should be in the list
        if (NodeSequence.IndexOf(node) == -1)
        {
            throw new Exception("Node not found in sequence");
        }

        ExplicitNextNode = node;
    }

    public T FirstNode => NodeSequence.First();

    public void Reinitalise()
    {
        _lastReturnedIndex = -1;
        ExplicitNextNode = default;
    }

    public T? GetNextInSequenceFromNodeOrNull(T node)
    {
        // Record the explicit next node 
        // This can wipe an existing explicit next node as we are redefining it
        SetExplicitNextNode(node);

        return GetNextInSequenceOrNull();
    }

    public T? GetNextInSequenceOrNull()
    {
        // Empty the explicit next node and return it
        if (ExplicitNextNode != null)
        {
            // Set the last index to the index of the explicit next node
            _lastReturnedIndex = NodeSequence.IndexOf(ExplicitNextNode);

            // Swap out the explicit next node so that it can still be returned
            var temp = ExplicitNextNode;
            ExplicitNextNode = default;

            return temp;
        }

        // We select the next in the sequence according to the recorded index
        _lastReturnedIndex++;

        if (_lastReturnedIndex >= NodeSequence.Count)
            return default;

        // Return the selected node
        return NodeSequence[_lastReturnedIndex];
    }

}
