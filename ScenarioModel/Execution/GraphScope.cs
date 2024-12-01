using ScenarioModel.Collections;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.Execution;

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
        CurrentNode = graph.PrimarySubGraph.NodeSequence.FirstOrDefault();
    }

    public IScenarioNode? GetNextInSequence(IScenarioNode node)
    {
        CurrentNode = CurrentSubGraph.GetNextInSequence(node);

        if (CurrentNode is not null)
        {
            return CurrentNode;
        }

        if (CurrentSubGraph.ParentSubGraph is null)
        {
            return null;
        }

        if (CurrentSubGraph.ParentSubGraphReentryPoint is null)
        {
            throw new InvalidOperationException("Incorherence : ParentSubGraphEntryPoint was null while ParentSubGraph was not");
        }

        // Go back up one subgraph and continue to the next node after the departure point
        var parentSubGraphReentryNode = CurrentSubGraph.ParentSubGraphReentryPoint;
        CurrentSubGraph = CurrentSubGraph.ParentSubGraph;

        // Here we have the node that entered the subgraph (the if node or the while node for example)
        // An if node must not be re-executed, but a while node should be
        // It's not appropriate that the GraphScope makes this decision
        if (parentSubGraphReentryNode is WhileNode)
        {
            CurrentNode = parentSubGraphReentryNode;
        }
        else
        {
            CurrentNode = CurrentSubGraph.GetNextInSequence(parentSubGraphReentryNode);
        }

        return CurrentNode;
    }

    public void EnterSubGraph(SemiLinearSubGraph<IScenarioNode> subGraph)
    {
        CurrentSubGraph = subGraph;
        CurrentNode = subGraph.NodeSequence.FirstOrDefault();
    }

    public void EnterSubGraphOnNode(SemiLinearSubGraph<IScenarioNode> subGraph, IScenarioNode node)
    {
        if (!subGraph.NodeSequence.Contains(node))
            throw new Exception("Node not found in subgraph");

        CurrentSubGraph = subGraph;
        CurrentNode = node;
    }
}
