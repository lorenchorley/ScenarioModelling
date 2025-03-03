using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.Execution;

// Assertions in C# code that works directly on the object model defined by the system and the MetaStory?
// Time series graphs and sequence diagrams derived from the MetaStory ?
// Component diagrams derived from the system?
// Code skeltons generated from the system?
// State exhaustiveness and reachability analysis?

// What makes relations special?

public class GraphScope
{
    public IStoryNode? CurrentNode { get; set; } // Null signifies the end of the graph
    public DirectedGraph<IStoryNode> Graph { get; }
    public Stack<SemiLinearSubGraphScope<IStoryNode>> SubGraphScopeStack { get; private set; } = new();
    public SemiLinearSubGraph<IStoryNode> CurrentSubGraph => CurrentSubGraphScope.Subgraph;
    public SemiLinearSubGraphScope<IStoryNode> CurrentSubGraphScope => SubGraphScopeStack.Peek();

    public GraphScope(DirectedGraph<IStoryNode> graph)
    {
        Graph = graph;
        SubGraphScopeStack.Push(Graph.PrimarySubGraph.GenerateScope(null));
        CurrentNode = CurrentSubGraphScope.MoveToNextInSequence();
    }

    public void SetExplicitNextNodeInSubGraph(IStoryNode node)
    {
        CurrentSubGraphScope.SetExplicitNextNodeInSubGraph(node);
    }

    public IStoryNode? MoveToNextInSequence()
    {
        CurrentNode = CurrentSubGraphScope.MoveToNextInSequence();

        if (CurrentNode is not null)
        {
            return CurrentNode;
        }

        if (CurrentSubGraphScope.ParentScope is null)
        {
            // Above this we need to manage the case where there is another graph is the stack.
            return null;
        }

        RemoveSubgraphFromStack();

        return MoveToNextInSequence(); // Explicit reentry point is handled by this method if set
    }

    // TODO validate
    public void EnterSubGraph(SemiLinearSubGraph<IStoryNode> subGraph)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(CurrentNode);

        if (CurrentNode is WhileNode)
            CurrentSubGraphScope.SetExplicitNextNodeInSubGraph(CurrentNode);

        AddSubgraphToStack(subGraph);

        CurrentNode = CurrentSubGraphScope.MoveToNextInSequence();
    }

    public void EnterSubGraphOnNode(SemiLinearSubGraph<IStoryNode> subGraph, IStoryNode node)
    {
        if (!subGraph.NodeSequence.Contains(node))
            throw new Exception("Node not found in subgraph");

        AddSubgraphToStack(subGraph);
        CurrentNode = node;
    }

    private void AddSubgraphToStack(SemiLinearSubGraph<IStoryNode> subGraph)
    {
        SubGraphScopeStack.Push(subGraph.GenerateScope(CurrentSubGraphScope));
    }

    private void RemoveSubgraphFromStack()
    {
        SubGraphScopeStack.Pop();
    }
}
