using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CodeHooks.Utils;

public class ActiveHookFunctions : InactiveHookFunctions
{
    private readonly Stack<SubgraphScopedHookSynchroniser> _scopeStack;
    private readonly Queue<IStoryNode> _contextBuilderInputs;
    private readonly HookContextBuilder _contextBuilder;
    private readonly ParallelConstructionExecutor? _parallelConstructionExecutor;

    public ActiveHookFunctions(Stack<SubgraphScopedHookSynchroniser> scopeStack, Queue<INodeHookDefinition> newlyCreatedHooks, Queue<IStoryNode> contextBuilderInputs, HookContextBuilder contextBuilder, ParallelConstructionExecutor? parallelConstructionExecutor)
        : base(newlyCreatedHooks)
    {
        _scopeStack = scopeStack;
        _contextBuilderInputs = contextBuilderInputs;
        _contextBuilder = contextBuilder;
        _parallelConstructionExecutor = parallelConstructionExecutor;
    }

    public override SubgraphScopedHookSynchroniser EnterSubgraph(SemiLinearSubGraph<IStoryNode> subgraph)
    {
        VerifyPreviousDefinition();

        var scopeSynchroniser = new SubgraphScopedHookSynchroniser(subgraph, VerifyPreviousDefinition);

        _scopeStack.Push(scopeSynchroniser);

        return scopeSynchroniser;
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

                _contextBuilderInputs.Enqueue(newNode); // TODO Remove the inputs class, it's too much
                _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);

                ArgumentNullExceptionStandard.ThrowIfNull(_parallelConstructionExecutor);
                //_parallelConstructionExecutor!.AddNodeToStoryAndAdvance(newNode); // TODO This generates an extra event sometimes after RegisterEventForHook has already created one. It's not clear which should win out and when
            },
            existing: hookDefinition.ReplaceNodeWithExisting
        );
    }

    public override void RegisterEventForHook(INodeHookDefinition hookDefinition, Action<IMetaStoryEvent> configure)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_parallelConstructionExecutor);

        var node = hookDefinition.GetNode();
        var @event = _parallelConstructionExecutor!.GenerateEvent(node);
        configure(@event);

        _parallelConstructionExecutor!.RegisterEvent(@event); // TODO This generates an extra event sometimes after FinaliseDefintion has already created one. It's not clear which should win out and when
    }

}
