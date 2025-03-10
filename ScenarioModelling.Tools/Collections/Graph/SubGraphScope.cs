namespace ScenarioModelling.Tools.Collections.Graph;

public class SemiLinearSubGraphScope<T> : ISubGraphScope<T> where T : IDirectedGraphNode<T>
{
    public SemiLinearSubGraph<T> Subgraph { get; private set; }
    public ISubGraphScope<T>? ParentScope { get; private set; } // Null signifies the highest level
    public int LastReturnedIndex { get; set; } = -1;
    public T? ExplicitNextNode { get; set; } = default;

    public SemiLinearSubGraphScope(SemiLinearSubGraph<T> subgraph, ISubGraphScope<T>? parentScope)
    {
        Subgraph = subgraph;
        ParentScope = parentScope;
    }

    public void SetExplicitNextNodeInSubGraph(T node)
    {
        // Node should be in the list
        if (Subgraph.NodeSequence.IndexOf(node) == -1)
        {
            throw new Exception("Node not found in sequence");
        }

        ExplicitNextNode = node;
    }

    public void Reinitalise()
    {
        LastReturnedIndex = -1;
        ExplicitNextNode = default;
    }

    public T? MoveToNextInSequenceFromNode(T node)
    {
        // Record the explicit next node 
        // This can wipe an existing explicit next node as we are redefining it
        SetExplicitNextNodeInSubGraph(node);

        return MoveToNextInSequence();
    }

    public T? MoveToNextInSequence()
    {
        // Empty the explicit next node and return it
        if (ExplicitNextNode != null)
        {
            // Set the last index to the index of the explicit next node
            LastReturnedIndex = Subgraph.NodeSequence.IndexOf(ExplicitNextNode);

            // Swap out the explicit next node so that it can still be returned
            var temp = ExplicitNextNode;
            ExplicitNextNode = default;

            return temp;
        }

        // We select the next in the sequence according to the recorded index
        LastReturnedIndex++;

        if (LastReturnedIndex >= Subgraph.NodeSequence.Count)
            return default;

        // Return the selected node
        return Subgraph.NodeSequence[LastReturnedIndex];
    }

}