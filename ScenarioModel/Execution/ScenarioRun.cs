using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Execution.Events.Interfaces;
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

    public IScenarioNode? GetNextInSequence(IScenarioNode? node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        CurrentNode = CurrentSubGraph.GetNextInSequence(node);

        if (CurrentNode is not null)
        {
            return CurrentNode;
        }

        if (CurrentSubGraph.ParentSubGraph is null)
        {
            return null;
        }

        if (CurrentSubGraph.ParentSubGraphEntryPoint is null)
        {
            throw new InvalidOperationException("Incorherence : ParentSubGraphEntryPoint was null while ParentSubGraph was not");
        }

        // Go back up one subgraph and continue to the next node after the departure point
        var parentSubGraphRentryNode = CurrentSubGraph.ParentSubGraphEntryPoint;
        CurrentSubGraph = CurrentSubGraph.ParentSubGraph;
        CurrentNode = CurrentSubGraph.GetNextInSequence(parentSubGraphRentryNode);

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

/// <summary>
/// A story is an instance or a play through of a scenario.
/// </summary>
public class ScenarioRun
{
    public Scenario Scenario { get; init; } = null!;
    public List<IScenarioEvent> Events { get; set; } = new();
    public Stack<GraphScope> GraphScopeStack { get; set; } = new();

    public void Init()
    {
        GraphScopeStack.Push(new GraphScope(Scenario.Graph));
    }

    public IScenarioNode? NextNode()
    {
        if (GraphScopeStack.Count == 0)
            return null;

        var currentEvent = Events.LastOrDefault();
        var currentScopeNode = GraphScopeStack.Peek().CurrentNode;

        if (currentScopeNode is null)
            return AdvanceToNextNodeInSubgraph(currentScopeNode);

        return currentScopeNode.ToOneOf().Match(
            (ChooseNode node) => ManangeChoseNode(currentEvent, node),
            (DialogNode node) => AdvanceToNextNodeInSubgraph(currentScopeNode),
            (IfNode node) => ManageIfNode(currentEvent, node),
            (JumpNode node) => ManageJumpNode(currentEvent, node),
            (TransitionNode node) => ManangeTransitionNode(currentEvent, node),
            (WhileNode node) => ManageWhileNode(currentEvent, node)
            );
    }

    private IScenarioNode? AdvanceToNextNodeInSubgraph(IScenarioNode? currentScopeNode)
    {
        return GraphScopeStack.Peek().GetNextInSequence(currentScopeNode);
    }

    private IScenarioNode? ManangeTransitionNode(IScenarioEvent? currentEvent, TransitionNode currentScopeNode)
    {
        // The last event must be a state change event
        if (currentEvent is null ||
            currentEvent is not StateChangeEvent stateChangeEvent)
            throw new Exception($"No {nameof(stateChangeEvent)} was registered after mananging a {nameof(StateChangeEvent)}");



        return AdvanceToNextNodeInSubgraph(currentScopeNode);
    }
    
    private IScenarioNode? ManangeChoseNode(IScenarioEvent? currentEvent, ChooseNode currentScopeNode)
    {
        // The last event must be a choice event
        if (currentEvent is null ||
            currentEvent is not ChoiceSelectedEvent choiceEvent)
            throw new Exception($"No {nameof(ChoiceSelectedEvent)} was registered after mananging a {nameof(ChooseNode)}");

        // Find the next node based on the choice
        IScenarioNode? newCurrentScopeNode =
            GraphScopeStack.Peek()
                           .Graph
                           .Find(s => s.Name.IsEqv(choiceEvent.Choice));

        if (newCurrentScopeNode is null)
            throw new Exception($@"ChooseNode attempted to jump to node ""{choiceEvent.Choice}"" but it was not present in the graph");

        GraphScopeStack.Peek().CurrentNode = newCurrentScopeNode;

        return newCurrentScopeNode;
    }

    private IScenarioNode? ManageJumpNode(IScenarioEvent? currentEvent, JumpNode currentScopeNode)
    {
        // The last event must be a jump event
        if (currentEvent is null ||
            currentEvent is not JumpEvent jumpEvent)
            throw new Exception($"No {nameof(JumpEvent)} was registered after mananging a {nameof(JumpNode)}");

        // Find the next node based on the choice
        IScenarioNode? newCurrentScopeNode =
            GraphScopeStack.Peek()
                           .Graph
                           .Find(s => s.Name.IsEqv(jumpEvent.Target));

        if (newCurrentScopeNode is null)
            throw new Exception($@"Node ""{jumpEvent.Target}"" not found in graph");

        GraphScopeStack.Peek().CurrentNode = newCurrentScopeNode;

        return newCurrentScopeNode;
    }

    private IScenarioNode? ManageIfNode(IScenarioEvent? currentEvent, IScenarioNode? currentScopeNode)
    {
        // The last event must be an if event
        if (currentEvent is null ||
            currentEvent is not IfBlockEvent ifEvent)
            throw new Exception($"No {nameof(IfBlockEvent)} was registered after mananging a {nameof(IfNode)}");

        if (ifEvent.IfBlockRun)
        {
            GraphScopeStack.Peek().EnterSubGraph(ifEvent.ProducerNode.SubGraph);
            return GraphScopeStack.Peek().CurrentNode; // Automatically the first node in the subgraph
        }
        else
        {
            // Otherwise advance past the if node
            return AdvanceToNextNodeInSubgraph(currentScopeNode);
        }
    }

    private IScenarioNode? ManageWhileNode(IScenarioEvent? currentEvent, WhileNode currentScopeNode)
    {
        // The last event must be an while event
        if (currentEvent is null ||
            currentEvent is not WhileLoopConditionCheckEvent whileEvent)
            throw new Exception($"No {nameof(WhileLoopConditionCheckEvent)} was registered after mananging a {nameof(IfNode)}");

        if (whileEvent.LoopBlockRun)
        {
            GraphScopeStack.Peek().EnterSubGraph(whileEvent.ProducerNode.SubGraph);
            return GraphScopeStack.Peek().CurrentNode; // Automatically the first node in the subgraph
        }
        else
        {
            // Otherwise advance past the while node
            return AdvanceToNextNodeInSubgraph(currentScopeNode);
        }
    }

    public void RegisterEvent(IScenarioEvent @event)
    {
        Events.Add(@event);
    }
}

public interface StoryRunResult
{
    public static StoryRunResult ConstraintFailure(string value) => new ConstraintFailure(value);
    public static StoryRunResult Successful(ScenarioRun story) => new Successful(story);
    public static StoryRunResult NotStarted() => new NotStarted();
}

public class ConstraintFailure(string value) : StoryRunResult
{
    public string Value { get; } = value;
}

public class Successful(ScenarioRun story) : StoryRunResult
{
    public ScenarioRun Story { get; } = story;
}

public class NotStarted : StoryRunResult
{
}