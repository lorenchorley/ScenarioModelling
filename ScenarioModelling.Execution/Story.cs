using LanguageExt;
using LanguageExt.SomeHelp;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution;

/// <summary>
/// A story is an instance or a play through of a MetaStory.
/// </summary>
public class Story
{
    public MetaStory MetaStory { get; set; } = null!;
    public List<IStoryEvent> Events { get; set; } = new();
    public Stack<GraphScope> GraphScopeStack { get; set; } = new();
    public ExpressionEvalator Evaluator { get; set; } = null!;

    public GraphScope CurrentScope => GraphScopeStack.Peek();

    public void Init()
    {
        GraphScopeStack.Push(new GraphScope(MetaStory.Graph));
    }

    public IStoryNode? NextNode()
    {
        var failedConstraintEvents =
            MetaStory.MetaState
                    .Constraints
                    .Choose(CheckConstraint)
                    .ToList();

        if (failedConstraintEvents.Any())
        {
            failedConstraintEvents.ForEach(RegisterEvent);
            return null;
        }

        if (GraphScopeStack.Count == 0)
            return null; // We're finished

        var currentEvent = Events.LastOrDefault(); // Null only on the first run through because no events have yet to be registered
        var currentScopeNode = CurrentScope.CurrentNode;

        bool isFirstNode = currentEvent is null;
        if (isFirstNode)
        {
            // On this first run, we return the first node quickly without advancing so that it can be processed in the loop
            // On the next run, we will have the same node but with an event so that it can then be processed and we can advance
            return CurrentScope.CurrentNode;
        }

        bool finishedSubgraph = currentScopeNode is null;
        if (finishedSubgraph)
        {
            // If we can keep going up, we go up one level
            GraphScopeStack.Pop();

            return NextNode();
        }

        currentScopeNode = currentScopeNode.ToOneOf().Match(
            (ChooseNode node) => ManangeChoseNode(currentEvent, node),
            (DialogNode node) => CurrentScope.GetNextInSequence(),
            (IfNode node) => ManageIfNode(currentEvent, node),
            (JumpNode node) => ManageJumpNode(currentEvent, node),
            (MetadataNode node) => CurrentScope.GetNextInSequence(),
            (TransitionNode node) => ManangeTransitionNode(currentEvent, node),
            (WhileNode node) => ManageWhileNode(currentEvent, node)
        );

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

    private IStoryNode? ManangeTransitionNode(IStoryEvent? currentEvent, TransitionNode currentScopeNode)
    {
        // The last event must be a state change event
        if (currentEvent is null ||
            currentEvent is not StateChangeEvent stateChangeEvent)
            throw new Exception($"No {nameof(stateChangeEvent)} was registered after mananging a {nameof(StateChangeEvent)}");

        return CurrentScope.GetNextInSequence();
    }

    private IStoryNode? ManangeChoseNode(IStoryEvent? currentEvent, ChooseNode currentScopeNode)
    {
        // The last event must be a choice event
        if (currentEvent is null ||
            currentEvent is not ChoiceSelectedEvent choiceEvent)
            throw new Exception($"No {nameof(ChoiceSelectedEvent)} was registered after mananging a {nameof(ChooseNode)}");

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

    private IStoryNode? ManageJumpNode(IStoryEvent? currentEvent, JumpNode currentScopeNode)
    {
        // The last event must be a jump event
        if (currentEvent is null ||
            currentEvent is not JumpEvent jumpEvent)
            throw new Exception($"No {nameof(JumpEvent)} was registered after mananging a {nameof(JumpNode)}");

        // Find the next node based on the choice
        IStoryNode? newCurrentScopeNode =
            CurrentScope
                .Graph
                .Find(s => s.Name.IsEqv(jumpEvent.Target));

        if (newCurrentScopeNode is null)
            throw new Exception($@"Node ""{jumpEvent.Target}"" not found in graph");

        // Set explicit next node
        CurrentScope.SetExplicitNextNode(newCurrentScopeNode);

        return CurrentScope.GetNextInSequence();
    }

    private IStoryNode? ManageIfNode(IStoryEvent? currentEvent, IStoryNode? currentScopeNode)
    {
        // The last event must be an if event
        if (currentEvent is null ||
            currentEvent is not IfBlockEvent ifEvent)
            throw new Exception($"No {nameof(IfBlockEvent)} was registered after mananging a {nameof(IfNode)}");

        if (ifEvent.IfBlockRun)
        {
            CurrentScope.EnterSubGraph(ifEvent.ProducerNode.SubGraph);
            return CurrentScope.CurrentNode; // Automatically the first node in the subgraph
        }
        else
        {
            // Otherwise advance past the if node
            return CurrentScope.GetNextInSequence();
        }
    }

    private IStoryNode? ManageWhileNode(IStoryEvent? currentEvent, WhileNode currentScopeNode)
    {
        // The last event must be an while event
        if (currentEvent is null ||
            currentEvent is not WhileLoopConditionCheckEvent whileEvent)
            throw new Exception($"No {nameof(WhileLoopConditionCheckEvent)} was registered after mananging a {nameof(IfNode)}");

        if (whileEvent.LoopBlockRun)
        {
            CurrentScope.EnterSubGraph(whileEvent.ProducerNode.SubGraph);
            return CurrentScope.CurrentNode; // Automatically the first node in the subgraph
        }
        else
        {
            // Otherwise advance past the while node
            return CurrentScope.GetNextInSequence();
        }
    }

    public void RegisterEvent(IStoryEvent @event)
    {
        Events.Add(@event);
    }
}

public class StoryRunResult
{
    public static StoryRunResult ConstraintFailure(string value) => new ConstraintFailure(value);
    public static StoryRunResult Successful(Story story) => new Successful(story);
    public static StoryRunResult NotStarted() => new NotStarted();
}

public class ConstraintFailure(string value) : StoryRunResult
{
    public string Value { get; } = value;
}

public class Successful(Story story) : StoryRunResult
{
    public Story Story { get; } = story;
}

public class NotStarted : StoryRunResult
{
}