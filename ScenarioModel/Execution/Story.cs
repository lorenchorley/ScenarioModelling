using LanguageExt;
using LanguageExt.SomeHelp;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Expressions.Evaluation;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.Objects.SystemObjects;

namespace ScenarioModelling.Execution;

/// <summary>
/// A story is an instance or a play through of a scenario.
/// </summary>
public class Story
{
    public MetaStory Scenario { get; set; } = null!;
    public List<IScenarioEvent> Events { get; set; } = new();
    public Stack<GraphScope> GraphScopeStack { get; set; } = new();
    public ExpressionEvalator Evaluator { get; set; } = null!;

    public GraphScope CurrentScope => GraphScopeStack.Peek();

    public void Init()
    {
        GraphScopeStack.Push(new GraphScope(Scenario.Graph));
    }

    public IScenarioNode? NextNode()
    {
        var failedConstraintEvents =
            Scenario.System
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
            return constraint.GenerateConstraintFailedEvent().ToSome();
        }

        return null;
    }

    private IScenarioNode? ManangeTransitionNode(IScenarioEvent? currentEvent, TransitionNode currentScopeNode)
    {
        // The last event must be a state change event
        if (currentEvent is null ||
            currentEvent is not StateChangeEvent stateChangeEvent)
            throw new Exception($"No {nameof(stateChangeEvent)} was registered after mananging a {nameof(StateChangeEvent)}");

        return CurrentScope.GetNextInSequence();
    }

    private IScenarioNode? ManangeChoseNode(IScenarioEvent? currentEvent, ChooseNode currentScopeNode)
    {
        // The last event must be a choice event
        if (currentEvent is null ||
            currentEvent is not ChoiceSelectedEvent choiceEvent)
            throw new Exception($"No {nameof(ChoiceSelectedEvent)} was registered after mananging a {nameof(ChooseNode)}");

        // Find the next node based on the choice
        IScenarioNode? newCurrentScopeNode =
            CurrentScope
                .Graph
                .Find(s => s.Name.IsEqv(choiceEvent.Choice));

        if (newCurrentScopeNode is null)
            throw new Exception($@"ChooseNode attempted to jump to node ""{choiceEvent.Choice}"" but it was not present in the graph");

        CurrentScope.CurrentNode = newCurrentScopeNode;

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
            CurrentScope
                .Graph
                .Find(s => s.Name.IsEqv(jumpEvent.Target));

        if (newCurrentScopeNode is null)
            throw new Exception($@"Node ""{jumpEvent.Target}"" not found in graph");

        // Set explicit next node
        CurrentScope.SetExplicitNextNode(newCurrentScopeNode);

        return CurrentScope.GetNextInSequence();
    }

    private IScenarioNode? ManageIfNode(IScenarioEvent? currentEvent, IScenarioNode? currentScopeNode)
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

    private IScenarioNode? ManageWhileNode(IScenarioEvent? currentEvent, WhileNode currentScopeNode)
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

    public void RegisterEvent(IScenarioEvent @event)
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