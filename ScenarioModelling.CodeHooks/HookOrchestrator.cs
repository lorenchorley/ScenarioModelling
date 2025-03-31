using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;
using ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;
using ScenarioModelling.CodeHooks.HookDefinitions.TestCaseObjects;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.MetaStoryNodes.DataClasses;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks;

public abstract class HookOrchestrator
{
    public Context Context { get; }

    protected readonly Stack<SubgraphScopedHookSynchroniser> _scopeStack = new();
    protected readonly Queue<IStoryNode> _contextBuilderInputs;
    protected readonly HookContextBuilder _contextBuilder;
    protected readonly Queue<INodeHookDefinition> _newlyCreatedHooks = new();
    protected readonly Instanciator _instanciator;
    private readonly MetaStoryDefinitionStack _metaStoryStack;
    protected readonly IServiceProvider _serviceProvider;

    protected MetaStateHookDefinition? _metaStateHookDefinition;
    protected readonly Stack<MetaStoryHookDefinition> _metaStoryDefintionStack = new();
    protected ParallelConstructionExecutor? _parallelConstructionExecutor;
    protected IMetaStoryHookFunctions _metaStoryHookFunctions;
    protected bool _throwOnConstraintOrAssertionFailure = false;
    protected bool _runStoryInParallelToHooks = false;

    protected SubgraphScopedHookSynchroniser CurrentScope => _scopeStack.Peek();

    protected HookOrchestrator(Context context, Instanciator instanciator, MetaStoryDefinitionStack metaStoryStack, IServiceProvider serviceProvider)
    {
        Context = context;
        _instanciator = instanciator;
        _metaStoryStack = metaStoryStack;
        _serviceProvider = serviceProvider;
        _contextBuilder = new(context);
        _contextBuilderInputs = new();

        _metaStoryHookFunctions = new InactiveHookFunctions(_newlyCreatedHooks);
    }

    /// <summary>
    /// Not in prod, but good for test to have exception close to pb
    /// </summary>
    public void ThrowOnConstraintOrAssertionFailure()
    {
        _throwOnConstraintOrAssertionFailure = true; // TODO Implement
    }
    
    /// <summary>
    /// Allows for running a story in parallel to the generation of a context via hooks
    /// </summary>
    public void RunStoryInParallelToHooks()
    {
        _runStoryInParallelToHooks = true; // TODO Implement
    }

    public virtual MetaStoryHookDefinition StartMetaStory(string name)
    {
        bool noMetaStoryInProgress = _metaStoryStack.Count == 0;

        if (noMetaStoryInProgress)
        {
            _metaStateHookDefinition = null; // Reinitialise so that it may be called again once per meta story

            MetaStoryHookDefinition metaStoryDefintion = new MetaStoryHookDefinition(name, Context, _metaStoryStack); // This pushes the new meta story onto the stack
            _metaStoryDefintionStack.Push(metaStoryDefintion);

            if (_metaStoryStack.Count != 1)
            {
                throw new InternalLogicException("MetaStoryHookDefinition did not push a meta story onto the stack");
            }

            MetaStory currentMetaStory = _metaStoryStack.Peek();

            if (_parallelConstructionExecutor != null)
                throw new InternalLogicException("ParallelConstructionExecutor was instancied before starting a new meta story without any on the stack");

            if (_runStoryInParallelToHooks)
                _parallelConstructionExecutor = _serviceProvider.GetRequiredService<ParallelConstructionExecutor>();

            _metaStoryHookFunctions = new ActiveHookFunctions(_scopeStack, _newlyCreatedHooks, _contextBuilderInputs, _contextBuilder, _parallelConstructionExecutor);

            _parallelConstructionExecutor?.StartMetaStory(name);
            _scopeStack.Push(new SubgraphScopedHookSynchroniser((SemiLinearSubGraph<IStoryNode>)currentMetaStory.Graph.PrimarySubGraph, _metaStoryHookFunctions.VerifyPreviousDefinition));
            _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs); // TODO Needed ?

            return metaStoryDefintion;
        }
        else
        {
            // TODO Needed ?
            //if (_metaStateHookDefinition == null)
            //    throw new HookException("A meta state has not been defined already");

            if (_metaStoryHookFunctions is not ActiveHookFunctions)
                throw new InternalLogicException("Hook functions are not active when starting a secondary meta story");

            _metaStoryHookFunctions.VerifyPreviousDefinition();

            MetaStoryHookDefinition metaStoryDefintion = new MetaStoryHookDefinition(name, Context, _metaStoryStack); // This pushes the new meta story onto the stack
            _metaStoryDefintionStack.Push(metaStoryDefintion);

            MetaStory currentMetaStory = _metaStoryStack.Peek(); // Must be after the creation of MetaStoryHookDefinition because it's that class that pushes the new meta story onto the stack

            if (_metaStoryStack.Count < 2)
                throw new InternalLogicException("MetaStoryHookDefinition did not push a meta story onto the stack.");

            if (_parallelConstructionExecutor == null && _runStoryInParallelToHooks)
                throw new InternalLogicException("ParallelConstructionExecutor was not instancied before starting a new meta story with existing meta stories on the stack");

            _parallelConstructionExecutor?.StartMetaStory(name);
            _scopeStack.Push(new SubgraphScopedHookSynchroniser((SemiLinearSubGraph<IStoryNode>)currentMetaStory.Graph.PrimarySubGraph, _metaStoryHookFunctions.VerifyPreviousDefinition));
            _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs); // TODO Needed ?

            return metaStoryDefintion;
        }
    }

    public (MetaStory, Story?) EndMetaStory()
    {
        // TODO fill the public story with the events that have been generated by the hooks
        _metaStoryHookFunctions.VerifyPreviousDefinition();

        if (_metaStoryStack.Count == 0)
            throw new InternalLogicException("When ending a meta story, the meta story stack count was 0, meaning no meta story had been started.");

        if (_metaStoryDefintionStack.Count == 0)
            throw new InternalLogicException("When ending a meta story, the meta story definition stack count was 0, meaning no meta story had been started.");

        if (_metaStoryStack.Count != _metaStoryDefintionStack.Count)
            throw new InternalLogicException("When ending a meta story, the meta story stack count did not match the meta story definition stack count. They should always be in sync.");

        MetaStory metaStory = _metaStoryDefintionStack.Pop().EndMetaStory(); // EndMetaStory does the Pop for the MetaStoryStack and returns that instance

        Story? story = _parallelConstructionExecutor?.EndMetaStory();

        bool noMetaStoryInProgress = _metaStoryStack.Count == 0;
        if (noMetaStoryInProgress)
        {
            // This is the end of the last meta story in the stack, finalise the state of the orchestrator
            _parallelConstructionExecutor = null;
            _metaStoryHookFunctions = new InactiveHookFunctions(_newlyCreatedHooks);
        }
        else
        {
            _scopeStack.Pop(); // Check that it's in phase with the others
        }

        return (metaStory, story);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="choices"></param>
    /// <remarks>
    /// Inserts the node as soon as the declaration is finalised
    /// </remarks>
    /// <returns></returns>
    public virtual AssertHookDefinition Assert(string expression)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        AssertHookDefinition nodeDef = new(CurrentScope, expression, _metaStoryHookFunctions);

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
    public virtual AssertHookDefinition Assert(string name, string expression)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        AssertHookDefinition nodeDef = new(CurrentScope, expression, _metaStoryHookFunctions);
        nodeDef.Node.Name = name;

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
    public virtual CallMetaStoryHookDefinition CallMetaStory(string name)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        CallMetaStoryHookDefinition nodeDef = new(CurrentScope, _metaStoryHookFunctions);
        nodeDef.Node.MetaStoryName = name;

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
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        ChooseHookDefinition nodeDef = new(CurrentScope, _metaStoryHookFunctions);
        nodeDef.Node.Choices.AddRange(choices);

        _newlyCreatedHooks.Enqueue(nodeDef);
        return nodeDef;
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
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(text);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions!.VerifyPreviousDefinition();

        DialogHookDefinition nodeDef = new(CurrentScope, text, _metaStoryHookFunctions);

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
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(character);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(text);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        DialogHookDefinition nodeDef =
            new DialogHookDefinition(CurrentScope, text, _metaStoryHookFunctions)
                .WithCharacter(character);

        // Parent subgraph is null here in MetaStoryWithWhileLoop_ConstructionTest !

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
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(condition);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        IfHookDefinition nodeDef = new(CurrentScope, condition, _metaStoryHookFunctions);

        // TODO Get the existing node at this point if it exists and return it so that everything is update to date as soon as possible. Otherwise the subgraph is not the correct subgraph going forward
        // May not be possible as we don't have enough information to completely identify the definition at this point

        _newlyCreatedHooks.Enqueue(nodeDef); // TODO This is not the correct place to add the node to the queue, move to the coniditon hook
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
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(target);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        JumpHookDefinition nodeDef = new(CurrentScope, target, _metaStoryHookFunctions);

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
    public virtual LoopHookDefinition Loop()
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        LoopHookDefinition nodeDef = new(CurrentScope, _metaStoryHookFunctions);

        _newlyCreatedHooks.Enqueue(nodeDef); // TODO This is not the correct place to add the node to the queue, move to the coniditon hook
        return nodeDef;
    }

    /// <summary>
    /// Declare a etadata node in the highest subgraph only.
    /// </summary>
    /// <param name="text"></param>
    /// <remarks>
    /// Inserts the node as soon as the declaration is finalised
    /// </remarks>
    /// <returns></returns>
    public virtual MetadataHookDefinition Metadata(string text)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(text);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions!.VerifyPreviousDefinition();

        MetadataHookDefinition nodeDef = new(CurrentScope, text, _metaStoryHookFunctions);

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
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(statefulObjectName);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(transition);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        TransitionHookDefinition nodeDef = new(CurrentScope, Context.MetaState, statefulObjectName, transition, _metaStoryHookFunctions);

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
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);
        ArgumentExceptionStandard.ThrowIfNullOrEmpty(condition);

        if (_scopeStack.Count == 0)
            throw new HookException("No meta story in progress");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        WhileHookDefinition nodeDef = new(CurrentScope, condition, _metaStoryHookFunctions);

        _newlyCreatedHooks.Enqueue(nodeDef); // TODO This is not the correct place to add the node to the queue, move to the coniditon hook
        return nodeDef;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configure"></param>
    /// <exception cref="Exception"></exception>
    public void DefineMetaState(Action<MetaStateHookDefinition> configure)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);

        if (_scopeStack.Count != 0)
            throw new HookException("A meta story must not be in progress when defining the meta state");

        _metaStoryHookFunctions.VerifyPreviousDefinition();

        if (_metaStateHookDefinition != null)
            throw new Exception("DefineMetaState may only be called once per meta story, use ReconfigureMetaState instead to modify certain elements");

        _metaStateHookDefinition = new(Context, _instanciator);
        configure(_metaStateHookDefinition);
        _metaStateHookDefinition.Initialise();

        _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configure"></param>
    /// <exception cref="Exception"></exception>
    public void ReconfigureMetaState(Action<MetaStateHookReconfigurationDefinition> configure)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStoryHookFunctions);
        _metaStoryHookFunctions.VerifyPreviousDefinition();

        if (_metaStateHookDefinition == null)
            throw new Exception("DefineMetaState must be called before the MetaState can be reconfigured");

        MetaStateHookReconfigurationDefinition metaStateHookReconfigurationDefinition = new(_metaStateHookDefinition);
        configure(metaStateHookReconfigurationDefinition);
        _metaStateHookDefinition.Initialise();

        _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);
    }

    public TestCaseHookDefinition TestCase(string name, string metaStoryName)
    {
        TestCaseHookDefinition hookDefinition = new(name, Context, metaStoryName);

        return hookDefinition;
    }
    
    public void ResetToInitialState()
    {
        _parallelConstructionExecutor?.ResetToInitialState();
    }

}