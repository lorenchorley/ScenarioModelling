using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Tools.GenericInterfaces;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Execution;

public static class NodeExtensions
{
    public static IMetaStoryEvent GenerateEvent(this IStoryNode node, EventGenerationDependencies dependencies)
        => node.ToOneOf().Match<IMetaStoryEvent>(
                assert => assert.InternalGenerateEvent(dependencies),
                callMetaStory => callMetaStory.InternalGenerateEvent(dependencies),
                chooseNode => chooseNode.InternalGenerateEvent(dependencies),
                dialogNode => dialogNode.InternalGenerateEvent(dependencies),
                ifNode => ifNode.InternalGenerateEvent(dependencies),
                jumpNode => jumpNode.InternalGenerateEvent(dependencies),
                loopNode => loopNode.InternalGenerateEvent(dependencies),
                metadataNode => metadataNode.InternalGenerateEvent(dependencies),
                transitionNode => transitionNode.InternalGenerateEvent(dependencies),
                whileNode => whileNode.InternalGenerateEvent(dependencies)
            );

    public static AssertionEvent InternalGenerateEvent(this AssertNode node, EventGenerationDependencies dependencies)
    {
        return new AssertionEvent() { 
            Expression = node.OriginalExpressionText,
            ProducerNode = node 
        };
    }
    
    public static MetaStoryCalledEvent InternalGenerateEvent(this CallMetaStoryNode node, EventGenerationDependencies dependencies)
    {
        return new MetaStoryCalledEvent() { 
            Name = node.MetaStoryName,
            ProducerNode = node 
        };
    }

    public static ChoiceSelectedEvent InternalGenerateEvent(this ChooseNode node, EventGenerationDependencies dependencies)
    {
        return new ChoiceSelectedEvent() { ProducerNode = node };
    }

    public static DialogEvent InternalGenerateEvent(this DialogNode node, EventGenerationDependencies dependencies)
    {
        DialogEvent e = new DialogEvent()
        {
            Character = node.Character,
            ProducerNode = node,
        };

        string text = node.TextTemplate;
        text = dependencies.Interpolator.ReplaceInterpolations(text);
        e.Text = text;

        return e;
    }

    public static IfConditionCheckEvent InternalGenerateEvent(this IfNode node, EventGenerationDependencies dependencies)
    {
        IfConditionCheckEvent e = new IfConditionCheckEvent() { ProducerNode = node };

        var result = node.AssertionExpression.Accept(dependencies.Evaluator);

        if (result is not bool shouldExecuteBlock)
        {
            throw new Exception($"If condition {node.AssertionExpression} did not evaluate to a boolean, this is a failure of the expression validation mecanism to not correctly determine the return type.");
        }

        e.Expression = node.OriginalConditionText;
        e.IfBlockRun = shouldExecuteBlock;

        return e;
    }

    public static JumpEvent InternalGenerateEvent(this JumpNode node, EventGenerationDependencies dependencies)
    {
        return new JumpEvent()
        {
            Target = node.Target,
            ProducerNode = node,
        };
    }

    public static LoopEvent InternalGenerateEvent(this LoopNode node, EventGenerationDependencies dependencies)
    {
        LoopEvent e = new()
        {
            ProducerNode = node,
        };

        return e;
    }

    public static MetadataEvent InternalGenerateEvent(this MetadataNode node, EventGenerationDependencies dependencies)
    {
        throw new NotImplementedException();
    }

    public static StateChangeEvent InternalGenerateEvent(this TransitionNode node, EventGenerationDependencies dependencies)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(node.StatefulObject);

        StateChangeEvent e = new()
        {
            ProducerNode = node,
            StatefulObject = node.StatefulObject!,
            TransitionName = node.TransitionName
        };

        IStateful statefulObject =
            node.StatefulObject
                ?.ResolveReference()
                .Match(
                      Some: obj => obj,
                      None: () => throw new ExecutionException("Stateful object not found")
                  )
                ?? throw new InternalLogicException("StatefulObject was null");

        if (statefulObject.State == null)
        {
            if (statefulObject is IIdentifiable nameful)
                throw new Exception($"Attempted state transition {node.TransitionName} on {nameful.Name} but no state set initially");
            else
                throw new Exception($"Attempted state transition {node.TransitionName} on object but no state set initially");
        }

        var resolvedValue = statefulObject.State.ResolvedValue
                            ?? throw new Exception("Stateful object state is not set.");

        e.InitialState = new StateReference(dependencies.Context.MetaState) { Name = resolvedValue.Name };

        resolvedValue.DoTransition(node.TransitionName, statefulObject);

        resolvedValue = statefulObject.State.ResolvedValue
                        ?? throw new Exception("Stateful object state is not set.");

        e.FinalState = new StateReference(dependencies.Context.MetaState) { Name = resolvedValue.Name };

        return e;
    }

    public static WhileConditionCheckEvent InternalGenerateEvent(this WhileNode node, EventGenerationDependencies dependencies)
    {
        WhileConditionCheckEvent e = new()
        {
            ProducerNode = node,
        };

        var result = node.AssertionExpression.Accept(dependencies.Evaluator);

        if (result is not bool shouldExecuteBlock)
        {
            throw new Exception($"While loop condition {node.AssertionExpression} did not evaluate to a boolean, this is a failure of the expression validation mecanism to not correctly determine the return type.");
        }

        e.Expression = node.OriginalConditionText;
        e.LoopBlockRun = shouldExecuteBlock;

        return e;
    }

}
