using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.Execution;

// Assertions in C# code that works directly on the object model defined by the system and the scenario?
// Time series graphs and sequence diagrams derived from the scenario ?
// Component diagrams derived from the system?
// Code skeltons generated from the system?
// State exhaustiveness and reachability analysis?

// What makes relations special?

public class GraphScope
{
    public IScenarioNode? CurrentNode { get; set; }
    public SemiLinearSubGraph<IScenarioNode> CurrentSubGraph { get; set; }
    public DirectedGraph<IScenarioNode> Graph { get; }

    public GraphScope(DirectedGraph<IScenarioNode> graph)
    {
        Graph = graph;
        CurrentSubGraph = graph.PrimarySubGraph;
        CurrentNode = graph.PrimarySubGraph.GetNextInSequenceOrNull();
    }

    public void SetExplicitNextNode(IScenarioNode node)
    {
        CurrentSubGraph.SetExplicitNextNode(node);
    }

    public IScenarioNode? GetNextInSequence()
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

    public void EnterSubGraph(SemiLinearSubGraph<IScenarioNode> subGraph)
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

    public void EnterSubGraphOnNode(SemiLinearSubGraph<IScenarioNode> subGraph, IScenarioNode node)
    {
        if (!subGraph.NodeSequence.Contains(node))
            throw new Exception("Node not found in subgraph");

        CurrentSubGraph = subGraph;
        CurrentNode = node;
    }
}
