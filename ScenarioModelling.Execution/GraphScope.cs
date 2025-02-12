using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
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
    public IStoryNode? CurrentNode { get; set; }
    public SemiLinearSubGraph<IStoryNode> CurrentSubGraph { get; set; }
    public DirectedGraph<IStoryNode> Graph { get; }

    public GraphScope(DirectedGraph<IStoryNode> graph)
    {
        Graph = graph;
        CurrentSubGraph = graph.PrimarySubGraph;
        graph.Reinitalise();
        CurrentNode = graph.PrimarySubGraph.GetNextInSequenceOrNull();
    }

    //public IStoryNode GetCurrentNode()
    //{

    //}

    public void SetExplicitNextNode(IStoryNode node)
    {
        CurrentSubGraph.SetExplicitNextNode(node);
    }

    public IStoryNode? GetNextInSequence()
    {
        CurrentNode = CurrentSubGraph.GetNextInSequenceOrNull();

        if (CurrentNode is not null)
        {
            return CurrentNode;
        }

        if (CurrentSubGraph.ParentSubgraph is null)
        {
            return null; // End of graph
        }

        // Go back up one subgraph and continue to the next node after the departure point
        CurrentSubGraph = CurrentSubGraph.ParentSubgraph;
        return GetNextInSequence(); // Explicit reentry point is handled by this method if set
    }

    public void EnterSubGraph(SemiLinearSubGraph<IStoryNode> subGraph)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(CurrentNode);

        subGraph.ParentSubgraph = CurrentSubGraph;

        if (CurrentNode is WhileNode)
            CurrentSubGraph.SetExplicitNextNode(CurrentNode);

        // Reinitialise the subgraph so that we start at the beginning again if the subgraph has already been run
        subGraph.Reinitalise();

        CurrentSubGraph = subGraph;
        CurrentNode = subGraph.GetNextInSequenceOrNull();
    }

    public void EnterSubGraphOnNode(SemiLinearSubGraph<IStoryNode> subGraph, IStoryNode node)
    {
        if (!subGraph.NodeSequence.Contains(node))
            throw new Exception("Node not found in subgraph");

        CurrentSubGraph = subGraph;
        CurrentNode = node;
    }
}
