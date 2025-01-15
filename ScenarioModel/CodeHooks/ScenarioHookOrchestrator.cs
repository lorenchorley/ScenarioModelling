using ScenarioModelling.CodeHooks.HookDefinitions;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.Objects.ScenarioNodes.DataClasses;

namespace ScenarioModelling.CodeHooks;

public delegate void ReturnOneScopeLevelDelegate();
public delegate void EnterScopeDelegate(DefinitionScope scope);

public abstract class ScenarioHookOrchestrator
{
    public Context Context { get; }

    protected ScenarioHookDefinition? _scenarioDefintion;
    protected readonly Stack<DefinitionScope> _scopeStack = new();
    protected readonly HookContextBuilderInputs _contextBuilderInputs;
    protected readonly ProgressiveHookBasedContextBuilder _contextBuilder;
    protected readonly Queue<INodeHookDefinition> _newlyCreatedHooks = new();

    protected DefinitionScope CurrentScope => _scopeStack.Peek();
    protected System System => Scenario.System;
    protected MetaStory Scenario => _scenarioDefintion?.GetScenario() ?? throw new ArgumentNullException();

    protected ScenarioHookOrchestrator(Context context)
    {
        ScenarioNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeHookDefinition>();

        Context = context;
        _contextBuilder = new(context);
        _contextBuilderInputs = new();
    }

    private void ReturnOneScopeLevel()
    {
        VerifyPreviousDefinition();

        _scopeStack.Pop();
    }

    private void EnterNewScope(DefinitionScope scope)
    {
        VerifyPreviousDefinition();

        _scopeStack.Push(scope);
    }

    public virtual ScenarioHookDefinition? DeclareScenarioStart(string name)
    {
        _scenarioDefintion = new ScenarioHookDefinition(name, Context);

        _scopeStack.Push(new DefinitionScope(Scenario.Graph.PrimarySubGraph, VerifyPreviousDefinition));

        _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);
        return _scenarioDefintion;
    }

    public MetaStory DeclareScenarioEnd()
    {
        VerifyPreviousDefinition();

        // Validate all hooks
        //foreach (var hook in _newlyCreatedHooks)
        //{
        //    hook.ValidateFinalState();
        //}

        return _scenarioDefintion?.GetScenario() ?? throw new ArgumentNullException(nameof(_scenarioDefintion));
    }

    public virtual DialogHookDefinition DeclareDialog(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        VerifyPreviousDefinition();

        DialogHookDefinition nodeDef = new(CurrentScope, text, FinaliseDefintion);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual DialogHookDefinition DeclareDialog(string character, string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(character);
        ArgumentException.ThrowIfNullOrEmpty(text);

        VerifyPreviousDefinition();

        DialogHookDefinition nodeDef =
            new DialogHookDefinition(CurrentScope, text, FinaliseDefintion)
                .SetCharacter(character);

        // Parent subgraph is null here in ScenarioWithWhileLoop_ConstructionTest !

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual ChooseHookDefinition DeclareChoose(params ChoiceList choices)
    {
        VerifyPreviousDefinition();

        ChooseHookDefinition nodeDef = new(CurrentScope, FinaliseDefintion);
        nodeDef.Node.Choices.AddRange(choices);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual TransitionHookDefinition DeclareTransition(string statefulObjectName, string transition)
    {
        ArgumentException.ThrowIfNullOrEmpty(statefulObjectName);
        ArgumentException.ThrowIfNullOrEmpty(transition);

        VerifyPreviousDefinition();

        TransitionHookDefinition nodeDef = new(CurrentScope, Context.System, statefulObjectName, transition, FinaliseDefintion);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual IfHookDefinition DeclareIfBranch(string condition)
    {
        ArgumentException.ThrowIfNullOrEmpty(condition);

        VerifyPreviousDefinition();

        IfHookDefinition nodeDef = new(CurrentScope, condition, EnterNewScope, ReturnOneScopeLevel, VerifyPreviousDefinition, FinaliseDefintion);

        // TODO Get the existing node at this point if it exists and return it so that everything is update to date as soon as possible. Otherwise the subgraph is not the correct subgraph going forward
        // May not be possible as we don't have enough information to completely identify the definition at this point

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual JumpHookDefinition DeclareJump(string target)
    {
        ArgumentException.ThrowIfNullOrEmpty(target);

        VerifyPreviousDefinition();

        JumpHookDefinition nodeDef = new(CurrentScope, target, FinaliseDefintion);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual WhileHookDefinition DeclareWhileBranch(string condition)
    {
        ArgumentException.ThrowIfNullOrEmpty(condition);

        VerifyPreviousDefinition();

        WhileHookDefinition nodeDef = new(CurrentScope, condition, EnterNewScope, ReturnOneScopeLevel, VerifyPreviousDefinition, FinaliseDefintion);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public void DefineSystem(Action<SystemHookDefinition> configure)
    {
        VerifyPreviousDefinition();

        SystemHookDefinition systemHookDefinition = new(Context);
        configure(systemHookDefinition);
        systemHookDefinition.Initialise();

        _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);
    }

    /// <summary>
    /// Validate and add as we go that each node definition is correct so that 
    /// * the problem is raised as close to the definition as possible,
    /// * and so that each node is verified before the next is started.
    /// </summary>
    private void VerifyPreviousDefinition()
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

    private void FinaliseDefintion()
    {
        // Must be done after all properties have been set via the fluent API
        INodeHookDefinition previousDefinition = _newlyCreatedHooks.Dequeue();
        previousDefinition.Scope.AddOrVerifyInPhase(
            previousDefinition, 
            add: () =>
            {
                IScenarioNode newNode = previousDefinition.GetNode();
                _contextBuilderInputs.NewNodes.Enqueue(newNode);
                _contextBuilder.RefreshContextWithInputs(_contextBuilderInputs);
            },
            existing: previousDefinition.ReplaceNodeWithExisting
        );
    }

    private void ValidateDefinition(IHookDefinition definition)
    {
        if (definition.Validated)
            throw new Exception("Definition already validated");

        definition.Validate();
    }

}