using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.StoryNodes.DataClasses;

namespace ScenarioModelling.CodeHooks;

public class HookFunctions
{
    private readonly Stack<DefinitionScope> _scopeStack;
    private readonly Queue<INodeHookDefinition> _newlyCreatedHooks;
    private readonly HookContextBuilderInputs _contextBuilderInputs;
    private readonly ProgressiveHookBasedContextBuilder _contextBuilder;
    private readonly ParallelConstructionExecutor? _parallelConstructionExecutor;

    public HookFunctions(Stack<DefinitionScope> scopeStack, Queue<INodeHookDefinition> newlyCreatedHooks, HookContextBuilderInputs contextBuilderInputs, ProgressiveHookBasedContextBuilder contextBuilder, ParallelConstructionExecutor? parallelConstructionExecutor)
    {
        this._scopeStack = scopeStack;
        this._newlyCreatedHooks = newlyCreatedHooks;
        this._contextBuilderInputs = contextBuilderInputs;
        this._contextBuilder = contextBuilder;
        this._parallelConstructionExecutor = parallelConstructionExecutor;
    }

    public void EnterScope(DefinitionScope scope)
    {
        VerifyPreviousDefinition();

        _scopeStack.Push(scope);
    }

    public void ReturnOneScopeLevel()
    {
        VerifyPreviousDefinition();

        _scopeStack.Pop();
    }

    /// <summary>
    /// Validate and add as we go that each node definition is correct so that 
    /// * the problem is raised as close to the definition as possible,
    /// * and so that each node is verified before the next is started.
    /// </summary>
    public void VerifyPreviousDefinition()
    {
        if (_newlyCreatedHooks.Count == 0)
        {
            // Nothing to do
            return;
        }

        if (_newlyCreatedHooks.Count > 1)
            throw new Exception("Only one definition should have been create since the last call");

        INodeHookDefinition previousDefinition = _newlyCreatedHooks.Dequeue();

        if (!previousDefinition.Validated)
            throw new Exception("Previous definition was not validated, call the Build method to finalise the hook definition");
    }

    public void FinaliseDefinition(INodeHookDefinition hookDefinition)
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

    public void RegisterEventForHook(INodeHookDefinition hookDefinition, Action<IStoryEvent> configure)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_parallelConstructionExecutor);

        var node = hookDefinition.GetNode();
        var @event = _parallelConstructionExecutor!.GenerateEvent(node);
        configure(@event);

        _parallelConstructionExecutor!.RegisterEvent(@event); // TODO This generates an extra event sometimes after FinaliseDefintion has already created one. It's not clear which should win out and when
    }

}

public abstract class MetaStoryHookOrchestrator
{
    public Context Context { get; }

    protected readonly Stack<DefinitionScope> _scopeStack = new();
    protected readonly HookContextBuilderInputs _contextBuilderInputs;
    protected readonly ProgressiveHookBasedContextBuilder _contextBuilder;
    protected readonly Queue<INodeHookDefinition> _newlyCreatedHooks = new();

    private HookFunctions? _hookFunctions;
    protected SystemHookDefinition? _systemHookDefinition;
    protected MetaStoryHookDefinition? _metaStoryDefintion;
    protected ParallelConstructionExecutor? _parallelConstructionExecutor;

    protected DefinitionScope CurrentScope => _scopeStack.Peek();
    protected System System => MetaStory.System;
    protected MetaStory MetaStory => _metaStoryDefintion?.GetMetaStory() ?? throw new ArgumentNullException();

    protected MetaStoryHookOrchestrator(Context context)
    {
        MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeHookDefinition>();

        Context = context;
        _contextBuilder = new(context);
        _contextBuilderInputs = new();
    }

    public virtual MetaStoryHookDefinition? StartMetaStory(string name)
    {
        _systemHookDefinition = null; // Reinitialise so that it may be called again once per meta story
        
        _metaStoryDefintion = new MetaStoryHookDefinition(name, Context);
        _parallelConstructionExecutor = new(Context, new(Context.System));
        _hookFunctions = new HookFunctions(_scopeStack, _newlyCreatedHooks, _contextBuilderInputs, _contextBuilder, _parallelConstructionExecutor);

        _parallelConstructionExecutor.StartMetaStory(name);
        _scopeStack.Push(new DefinitionScope(MetaStory.Graph.PrimarySubGraph, _hookFunctions.VerifyPreviousDefinition));
        _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);

        return _metaStoryDefintion;
    }

    public (MetaStory, Story) EndMetaStory()
    {
        _hookFunctions.VerifyPreviousDefinition();

        // TODO fill the public story with the events that have been generated by the hooks
        ArgumentNullExceptionStandard.ThrowIfNull(_parallelConstructionExecutor);

        var story = _parallelConstructionExecutor!.EndMetaStory();
        _parallelConstructionExecutor = null;

        MetaStory metaStory = _metaStoryDefintion?.GetMetaStory() ?? throw new ArgumentNullException(nameof(_metaStoryDefintion));
        return (metaStory, story);
    }

    /// <summary>
    /// Declare a dialog node in the current subgraph.
    /// </summary>
    /// <param name="text"></param>
    /// <remarks>
    /// Inserts the node as soon as the declaration is finalised
    /// </remarks>
    /// <returns></returns>
    public virtual DialogHookDefinition Dialog(string text)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_hookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(text);

        _hookFunctions!.VerifyPreviousDefinition();

        DialogHookDefinition nodeDef = new(CurrentScope, text, _hookFunctions);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    /// <summary>
    /// Declare a dialog node in the current subgraph.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="text"></param>
    /// <remarks>
    /// Inserts the node as soon as the declaration is finalised
    /// </remarks>
    /// <returns></returns>
    public virtual DialogHookDefinition Dialog(string character, string text)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_hookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(character);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(text);

        _hookFunctions.VerifyPreviousDefinition();

        DialogHookDefinition nodeDef =
            new DialogHookDefinition(CurrentScope, text, _hookFunctions)
                .WithCharacter(character);

        // Parent subgraph is null here in MetaStoryWithWhileLoop_ConstructionTest !

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="choices"></param>
    /// <remarks>
    /// Inserts the node as soon as the declaration is finalised
    /// </remarks>
    /// <returns></returns>
    public virtual ChooseHookDefinition Choose(params ChoiceList choices)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_hookFunctions);

        _hookFunctions.VerifyPreviousDefinition();

        ChooseHookDefinition nodeDef = new(CurrentScope, _hookFunctions);
        nodeDef.Node.Choices.AddRange(choices);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="statefulObjectName"></param>
    /// <param name="transition"></param>
    /// <remarks>
    /// Inserts the node as soon as the declaration is finalised
    /// </remarks>
    /// <returns></returns>
    public virtual TransitionHookDefinition Transition(string statefulObjectName, string transition)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_hookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(statefulObjectName);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(transition);

        _hookFunctions.VerifyPreviousDefinition();

        TransitionHookDefinition nodeDef = new(CurrentScope, Context.System, statefulObjectName, transition, _hookFunctions);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <remarks>
    /// Inserts the node when the condition hook is used
    /// </remarks>
    /// <returns></returns>
    public virtual IfHookDefinition If(string condition)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_hookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(condition);

        _hookFunctions.VerifyPreviousDefinition();

        IfHookDefinition nodeDef = new(CurrentScope, condition, _hookFunctions);

        // TODO Get the existing node at this point if it exists and return it so that everything is update to date as soon as possible. Otherwise the subgraph is not the correct subgraph going forward
        // May not be possible as we don't have enough information to completely identify the definition at this point

        _newlyCreatedHooks.Enqueue(nodeDef); /// TODO This is not the correct place to add the node to the queue, move to the coniditon hook

        return nodeDef;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <remarks>
    /// Inserts the node as soon as the declaration is finalised
    /// </remarks>
    /// <returns></returns>
    public virtual JumpHookDefinition Jump(string target)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_hookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(target);

        _hookFunctions.VerifyPreviousDefinition();

        JumpHookDefinition nodeDef = new(CurrentScope, target, _hookFunctions);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <remarks>
    /// Inserts the node when the condition hook is used
    /// </remarks>
    /// <returns></returns>
    public virtual WhileHookDefinition While(string condition)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_hookFunctions);

        ArgumentExceptionStandard.ThrowIfNullOrEmpty(condition);

        _hookFunctions.VerifyPreviousDefinition();

        WhileHookDefinition nodeDef = new(CurrentScope, condition, _hookFunctions);

        _newlyCreatedHooks.Enqueue(nodeDef); /// TODO This is not the correct place to add the node to the queue, move to the coniditon hook

        return nodeDef;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configure"></param>
    /// <exception cref="Exception"></exception>
    public void DefineSystem(Action<SystemHookDefinition> configure)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_hookFunctions);

        _hookFunctions.VerifyPreviousDefinition();

        if (_systemHookDefinition != null)
            throw new Exception("DefineSystem may only be called once per meta story, use ReconfigureSystem instead to modify certain elements");

        _systemHookDefinition = new(Context);
        configure(_systemHookDefinition);
        _systemHookDefinition.Initialise();

        _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configure"></param>
    /// <exception cref="Exception"></exception>
    public void ReconfigureSystem(Action<SystemHookReconfigurationDefinition> configure)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_hookFunctions);

        _hookFunctions.VerifyPreviousDefinition();

        if (_systemHookDefinition == null)
            throw new Exception("DefineSystem must be called before the system can be reconfigured");

        SystemHookReconfigurationDefinition systemHookReconfigurationDefinition = new(_systemHookDefinition);
        configure(systemHookReconfigurationDefinition);
        _systemHookDefinition.Initialise();

        _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);
    }

}