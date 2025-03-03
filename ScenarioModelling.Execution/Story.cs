using LanguageExt;
using LanguageExt.SomeHelp;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Execution.EventSourcing;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Execution;

/// <summary>
/// A story is an instance or a play through of a MetaStory.
/// </summary>
public class Story
{
    public Context Context { get; }
    //public MetaStoryStack MetaStoryStack { get; }
    public ExpressionEvalator Evaluator { get; }

    //public MetaStory MetaStory => MetaStoryStack.Peek();
    public StoryEventSource Events { get; } = new();
    public Stack<GraphScope> GraphScopeStack { get; } = new();
    public GraphScope CurrentScope => GraphScopeStack.Peek();

    public Story(Context context/*, MetaStoryStack metaStoryStack*/, ExpressionEvalator evaluator)
    {
        Context = context;
        //MetaStoryStack = metaStoryStack;
        Evaluator = evaluator;
    }

    public void Init(MetaStory initialMetaStory/*, bool dontAddToStack = false*/)
    {
        //if (!dontAddToStack)
        //    MetaStoryStack.Push(initialMetaStory);

        // Reset
        Events.Events.Clear();
        GraphScopeStack.Clear();

        GraphScopeStack.Push(new GraphScope(initialMetaStory.Graph));
    }

    public IStoryNode? NextNode()
    {
        var failedConstraintEvents =
            Context.MetaState
                   .Constraints
                   .Choose(CheckConstraint)
                   .ToList();

        if (failedConstraintEvents.Any())
        {
            failedConstraintEvents.ForEach(RegisterEvent);
            return null;
        }

        if (GraphScopeStack.Count == 0)
            throw new Exception("should not be used, exception to mark the case");

        var currentEvent = Events.GetEnumerable().LastOrDefault(); // Null only on the first run through because no events have yet to be registered
        var currentScopeNode = CurrentScope.CurrentNode;

        bool isFirstNode = currentEvent is null;
        if (isFirstNode)
        {
            // On this first run, we return the first node quickly without advancing so that it can be processed in the loop
            // On the next run, we will have the same node but with an event so that it can then be processed and we can advance
            return CurrentScope.CurrentNode;
        }

        if (currentScopeNode is null)
        {
            throw new Exception("should not be used, exception to mark the case");
        }

        currentScopeNode = currentScopeNode.ToOneOf().Match(
            (CallMetaStoryNode node) => ManangeCallMetaStoryNode(currentEvent, node),
            (ChooseNode node) => ManangeChoseNode(currentEvent, node),
            (DialogNode node) => CurrentScope.MoveToNextInSequence(),
            (IfNode node) => ManageIfNode(currentEvent, node),
            (JumpNode node) => ManageJumpNode(currentEvent, node),
            (MetadataNode node) => CurrentScope.MoveToNextInSequence(),
            (TransitionNode node) => ManangeTransitionNode(currentEvent, node),
            (WhileNode node) => ManageWhileNode(currentEvent, node)
        );

        bool finishedSubgraph = currentScopeNode is null;
        if (finishedSubgraph)
        {
            // If we can keep going up, we go up one level
            GraphScopeStack.Pop();

            if (GraphScopeStack.Count == 0)
                return null; // Last graph has been popped, we're done

            Events.Add(new MetaStoryReturnedEvent());

            return CurrentScope.MoveToNextInSequence();
        }

        return currentScopeNode;
    }

    private Option<ConstraintFailedEvent> CheckConstraint(Constraint constraint)
    {
        var result = constraint.Condition.Accept(Evaluator);

        if (result is not bool shouldExecuteBlock)
        {
            throw new Exception($"If condition {constraint.OriginalConditionText} did not evaluate to a boolean, this is a failure of the expression validation mecanism to not correctly determine the return type.");
        }

        var constraintSatisfied = (bool)result;

        if (!constraintSatisfied)
        {
            return GenerateConstraintFailedEvent(constraint).ToSome();
        }

        return null;
    }

    public ConstraintFailedEvent GenerateConstraintFailedEvent(Constraint constraint)
    {
        return new ConstraintFailedEvent() { ProducerNode = constraint, Expression = constraint.OriginalConditionText };
    }

    private IStoryNode? ManangeTransitionNode(IMetaStoryEvent? currentEvent, TransitionNode currentScopeNode)
    {
        // The last event must be a state change event
        if (currentEvent is null ||
            currentEvent is not StateChangeEvent stateChangeEvent)
            throw new InternalLogicException($"No {nameof(stateChangeEvent)} was registered after mananging a {nameof(StateChangeEvent)}");

        return CurrentScope.MoveToNextInSequence();
    }

    private IStoryNode? ManangeCallMetaStoryNode(IMetaStoryEvent? currentEvent, CallMetaStoryNode currentScopeNode)
    {
        // The last event must be a choice event
        if (currentEvent is null ||
            currentEvent is not MetaStoryCalledEvent choiceEvent)
            throw new InternalLogicException($"No {nameof(MetaStoryCalledEvent)} was registered after mananging a {nameof(CallMetaStoryNode)}");

        var calledMetaStory = Context.MetaStories.Where(s => s.Name.IsEqv(currentScopeNode.MetaStoryName)).FirstOrDefault();
        if (calledMetaStory == null)
            throw new ExecutionException($"No meta story with the name {currentScopeNode.MetaStoryName} was found.");

        var currentGraph = GraphScopeStack.Peek();
        GraphScopeStack.Push(new GraphScope(calledMetaStory.Graph));
        //GraphScopeStack.Peek().Graph.PrimarySubGraph.ParentSubgraph = currentGraph;

        return CurrentScope.CurrentNode;
    }

    private IStoryNode? ManangeChoseNode(IMetaStoryEvent? currentEvent, ChooseNode currentScopeNode)
    {
        // The last event must be a choice event
        if (currentEvent is null ||
            currentEvent is not ChoiceSelectedEvent choiceEvent)
            throw new InternalLogicException($"No {nameof(ChoiceSelectedEvent)} was registered after mananging a {nameof(ChooseNode)}");

        // Find the next node based on the choice
        IStoryNode? newCurrentScopeNode =
            CurrentScope
                .Graph
                .Find(s => s.Name.IsEqv(choiceEvent.Choice));

        if (newCurrentScopeNode is null)
            throw new Exception($@"ChooseNode attempted to jump to node ""{choiceEvent.Choice}"" but it was not present in the graph");

        CurrentScope.CurrentNode = newCurrentScopeNode;

        return newCurrentScopeNode;
    }

    private IStoryNode? ManageJumpNode(IMetaStoryEvent? currentEvent, JumpNode currentScopeNode)
    {
        // The last event must be a jump event
        if (currentEvent is null ||
            currentEvent is not JumpEvent jumpEvent)
            throw new InternalLogicException($"No {nameof(JumpEvent)} was registered after mananging a {nameof(JumpNode)}");

        // Find the next node based on the choice
        IStoryNode? newCurrentScopeNode =
            CurrentScope
                .Graph
                .Find(s => s.Name.IsEqv(jumpEvent.Target));

        if (newCurrentScopeNode is null)
            throw new Exception($@"Node ""{jumpEvent.Target}"" not found in graph");

        // Set explicit next node
        CurrentScope.SetExplicitNextNodeInSubGraph(newCurrentScopeNode);

        return CurrentScope.MoveToNextInSequence();
    }

    private IStoryNode? ManageIfNode(IMetaStoryEvent? currentEvent, IStoryNode? currentScopeNode)
    {
        // The last event must be an if event
        if (currentEvent is null ||
            currentEvent is not IfConditionCheckEvent ifEvent)
            throw new InternalLogicException($"No {nameof(IfConditionCheckEvent)} was registered after mananging a {nameof(IfNode)}");

        if (ifEvent.IfBlockRun)
        {
            CurrentScope.EnterSubGraph(ifEvent.ProducerNode.SubGraph);
            return CurrentScope.CurrentNode; // Automatically the first node in the subgraph
        }
        else
        {
            // Otherwise advance past the if node
            return CurrentScope.MoveToNextInSequence();
        }
    }

    private IStoryNode? ManageWhileNode(IMetaStoryEvent? currentEvent, WhileNode currentScopeNode)
    {
        // The last event must be an while event
        if (currentEvent is null ||
            currentEvent is not WhileConditionCheckEvent whileEvent)
            throw new InternalLogicException($"No {nameof(WhileConditionCheckEvent)} was registered after mananging a {nameof(IfNode)}");

        if (whileEvent.LoopBlockRun)
        {
            CurrentScope.EnterSubGraph(whileEvent.ProducerNode.SubGraph);
            return CurrentScope.CurrentNode; // Automatically the first node in the subgraph
        }
        else
        {
            // Otherwise advance past the while node
            return CurrentScope.MoveToNextInSequence();
        }
    }

    public void RegisterEvent(IMetaStoryEvent @event)
    {
        Events.Add(@event);
    }
}
