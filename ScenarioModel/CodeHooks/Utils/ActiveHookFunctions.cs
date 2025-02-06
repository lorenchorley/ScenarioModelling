﻿using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.Utils;

public class ActiveHookFunctions : InactiveHookFunctions
{
    private readonly Stack<DefinitionScope> _scopeStack;
    private readonly HookContextBuilderInputs _contextBuilderInputs;
    private readonly ProgressiveHookBasedContextBuilder _contextBuilder;
    private readonly ParallelConstructionExecutor? _parallelConstructionExecutor;

    public ActiveHookFunctions(Stack<DefinitionScope> scopeStack, Queue<INodeHookDefinition> newlyCreatedHooks, HookContextBuilderInputs contextBuilderInputs, ProgressiveHookBasedContextBuilder contextBuilder, ParallelConstructionExecutor? parallelConstructionExecutor)
        : base(newlyCreatedHooks)
    {
        _scopeStack = scopeStack;
        _contextBuilderInputs = contextBuilderInputs;
        _contextBuilder = contextBuilder;
        _parallelConstructionExecutor = parallelConstructionExecutor;
    }

    public override void EnterScope(DefinitionScope scope)
    {
        VerifyPreviousDefinition();

        _scopeStack.Push(scope);
    }

    public override void ReturnOneScopeLevel()
    {
        VerifyPreviousDefinition();

        _scopeStack.Pop();
    }

    public override void FinaliseDefinition(INodeHookDefinition hookDefinition)
    {
        // Must be done after all properties have been set via the fluent API
        //INodeHookDefinition hookDefinition = _newlyCreatedHooks.Dequeue();
        hookDefinition.Scope.AddOrVerifyInPhase(
            hookDefinition,
            add: () =>
            {
                IStoryNode newNode = hookDefinition.GetNode();

                _contextBuilderInputs.NewNodes.Enqueue(newNode); // TODO Remove the inputs class, it's too much
                _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);

                ArgumentNullExceptionStandard.ThrowIfNull(_parallelConstructionExecutor);
                //_parallelConstructionExecutor!.AddNodeToStoryAndAdvance(newNode); // TODO This generates an extra event sometimes after RegisterEventForHook has already created one. It's not clear which should win out and when
            },
            existing: hookDefinition.ReplaceNodeWithExisting
        );
    }

    public override void RegisterEventForHook(INodeHookDefinition hookDefinition, Action<IStoryEvent> configure)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_parallelConstructionExecutor);

        var node = hookDefinition.GetNode();
        var @event = _parallelConstructionExecutor!.GenerateEvent(node);
        configure(@event);

        _parallelConstructionExecutor!.RegisterEvent(@event); // TODO This generates an extra event sometimes after FinaliseDefintion has already created one. It's not clear which should win out and when
    }

}
