using ScenarioModel.CodeHooks.HookDefinitions;
using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;

namespace ScenarioModel.CodeHooks;

public delegate void ReturnOneScopeLevelDelegate();
public delegate void EnterScopeDelegate(DefinitionScope scope);

public abstract class ScenarioHookOrchestrator
{
    public Context Context { get; }

    protected ScenarioHookDefinition? _scenarioDefintion;
    protected readonly Stack<DefinitionScope> _scopeStack = new();
    protected readonly HookContextBuilderInputs _contextBuilderInputs;
    protected readonly ProgressiveHookBasedContextBuilder _contextBuilder;
    protected readonly HashSet<INodeHookDefinition> _newlyCreatedHooks = new();

    protected DefinitionScope CurrentScope => _scopeStack.Peek();
    protected System System => Scenario.System;
    protected Scenario Scenario => _scenarioDefintion?.GetScenario() ?? throw new ArgumentNullException();

    protected ScenarioHookOrchestrator(Context context)
    {
        ScenarioNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<INodeHookDefinition>();

        Context = context;
        _contextBuilder = new(context);
        _contextBuilderInputs = new();
    }

    private void ReturnOneScopeLevel()
    {
        _scopeStack.Pop();
    }

    private void EnterNewScope(DefinitionScope scope)
    {
        _scopeStack.Push(scope);
    }

    public virtual ScenarioHookDefinition? DeclareScenarioStart(string name)
    {
        _scenarioDefintion = new ScenarioHookDefinition(name, Context);

        _scopeStack.Push(new DefinitionScope()
        {
            SubGraph = Scenario.Graph.PrimarySubGraph
        });

        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
        return _scenarioDefintion;
    }

    public Scenario DeclareScenarioEnd()
    {
        // Validate all hooks
        foreach (var hook in _newlyCreatedHooks)
        {
            hook.ValidateFinalState();
        }

        return _scenarioDefintion?.GetScenario() ?? throw new ArgumentNullException(nameof(_scenarioDefintion));
    }

    public virtual DialogHookDefinition DeclareDialog(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);

        DialogHookDefinition nodeDef = new(text);

        CurrentScope.AddOrVerifyInPhase(nodeDef, add: () =>
        {
            //_newlyCreatedHooks.Add(nodeDef); // TODO If exists already ?

            _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
            _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
        });

        return nodeDef;
    }

    public virtual DialogHookDefinition DeclareDialog(string character, string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(character);
        ArgumentException.ThrowIfNullOrEmpty(text);

        DialogHookDefinition nodeDef =
            new DialogHookDefinition(text)
                .SetCharacter(character);

        // Parent subgraph is null here in ScenarioWithWhileLoop_ConstructionTest !
        CurrentScope.AddOrVerifyInPhase(nodeDef, add: () => { });
        _newlyCreatedHooks.Add(nodeDef); // TODO If exists already ?

        _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

        return nodeDef;
    }

    public virtual ChooseHookDefinition DeclareChoose(params ChoiceList choices)
    {
        ChooseHookDefinition nodeDef = new();
        nodeDef.Node.Choices.AddRange(choices);

        CurrentScope.AddOrVerifyInPhase(nodeDef, add: () =>
        {
            //_newlyCreatedHooks.Add(nodeDef); // TODO If exists already ?

            _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
            _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
        });

        return nodeDef;
    }

    public virtual TransitionHookDefinition DeclareTransition(string statefulObjectName, string transition)
    {
        ArgumentException.ThrowIfNullOrEmpty(statefulObjectName);
        ArgumentException.ThrowIfNullOrEmpty(transition);

        TransitionHookDefinition nodeDef = new(Context.System, statefulObjectName, transition);

        CurrentScope.AddOrVerifyInPhase(nodeDef, add: () =>
        {
            //_newlyCreatedHooks.Add(nodeDef); // TODO If exists already ?

            // TODO Needs modifying to only happen when node is new ?
            _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
            _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
        });

        return nodeDef;
    }

    public virtual IfHookDefinition DeclareIfBranch(string condition)
    {
        ArgumentException.ThrowIfNullOrEmpty(condition);

        IfHookDefinition nodeDef = new(condition, EnterNewScope, ReturnOneScopeLevel);

        CurrentScope.AddOrVerifyInPhase(nodeDef, add: () =>
        {
            //_newlyCreatedHooks.Add(nodeDef); // TODO If exists already ?

            _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
            _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
        });

        return nodeDef;
    }

    public virtual JumpHookDefinition DeclareJump(string target)
    {
        ArgumentException.ThrowIfNullOrEmpty(target);

        JumpHookDefinition nodeDef = new(target);

        CurrentScope.AddOrVerifyInPhase(nodeDef, add: () =>
        {
            //_newlyCreatedHooks.Add(nodeDef); // TODO If exists already ?

            _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
            _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
        });

        return nodeDef;
    }

    public virtual WhileHookDefinition DeclareWhileBranch(string condition)
    {
        ArgumentException.ThrowIfNullOrEmpty(condition);

        WhileHookDefinition nodeDef = new(condition, CurrentScope, EnterNewScope, ReturnOneScopeLevel);

        _newlyCreatedHooks.Add(nodeDef); // TODO If exists already ?

        _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

        CurrentScope.AddOrVerifyInPhase(nodeDef, add: () =>
        {
            //_newlyCreatedHooks.Add(nodeDef); // TODO If exists already ?

            _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
            _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
        });

        return nodeDef;
    }

    public void DefineSystem(Action<SystemHookDefinition> configure)
    {
        SystemHookDefinition systemHookDefinition = new(Context);
        configure(systemHookDefinition);
        systemHookDefinition.Initialise();

        //_contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
    }
}