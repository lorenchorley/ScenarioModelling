using ScenarioModel.CodeHooks.HookDefinitions;
using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
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
    protected readonly Queue<INodeHookDefinition> _newlyCreatedHooks = new();

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
        VerifyAndAddLastDefinition();

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

        VerifyAndAddLastDefinition();

        DialogHookDefinition nodeDef = new(CurrentScope, text);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual DialogHookDefinition DeclareDialog(string character, string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(character);
        ArgumentException.ThrowIfNullOrEmpty(text);

        VerifyAndAddLastDefinition();

        DialogHookDefinition nodeDef =
            new DialogHookDefinition(CurrentScope, text)
                .SetCharacter(character);

        // Parent subgraph is null here in ScenarioWithWhileLoop_ConstructionTest !

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual ChooseHookDefinition DeclareChoose(params ChoiceList choices)
    {
        VerifyAndAddLastDefinition();

        ChooseHookDefinition nodeDef = new(CurrentScope);
        nodeDef.Node.Choices.AddRange(choices);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual TransitionHookDefinition DeclareTransition(string statefulObjectName, string transition)
    {
        ArgumentException.ThrowIfNullOrEmpty(statefulObjectName);
        ArgumentException.ThrowIfNullOrEmpty(transition);

        VerifyAndAddLastDefinition();

        TransitionHookDefinition nodeDef = new(CurrentScope, Context.System, statefulObjectName, transition);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual IfHookDefinition DeclareIfBranch(string condition)
    {
        ArgumentException.ThrowIfNullOrEmpty(condition);

        VerifyAndAddLastDefinition();

        IfHookDefinition nodeDef = new(CurrentScope, condition, EnterNewScope, ReturnOneScopeLevel);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual JumpHookDefinition DeclareJump(string target)
    {
        ArgumentException.ThrowIfNullOrEmpty(target);

        VerifyAndAddLastDefinition();

        JumpHookDefinition nodeDef = new(CurrentScope, target);

        _newlyCreatedHooks.Enqueue(nodeDef);

        return nodeDef;
    }

    public virtual WhileHookDefinition DeclareWhileBranch(string condition)
    {
        ArgumentException.ThrowIfNullOrEmpty(condition);

        VerifyAndAddLastDefinition();

        WhileHookDefinition nodeDef = new(CurrentScope, condition, EnterNewScope, ReturnOneScopeLevel);

        _newlyCreatedHooks.Enqueue(nodeDef);

        //_contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        //_contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

        return nodeDef;
    }

    public void DefineSystem(Action<SystemHookDefinition> configure)
    {
        VerifyAndAddLastDefinition();

        SystemHookDefinition systemHookDefinition = new(Context);
        configure(systemHookDefinition);
        systemHookDefinition.Initialise();

        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
    }

    /// <summary>
    /// Validate and add as we go that each node definition is correct so that 
    /// * the problem is raised as close to the definition as possible,
    /// * and so that each node is verified before the next is started.
    /// </summary>
    private void VerifyAndAddLastDefinition()
    {
        if (_newlyCreatedHooks.Count == 0)
        {
            // Nothing to do
            return;
        }

        if (_newlyCreatedHooks.Count > 1)
            throw new Exception("Only one definition should have been create since the last call");

        INodeHookDefinition lastDefinition = _newlyCreatedHooks.Dequeue();
        ValidateDefinition(lastDefinition);

        // Must be done after all properties have been set via the fluent API
        lastDefinition.CurrentScope.AddOrVerifyInPhase(lastDefinition, add: () =>
        {
            IScenarioNode newNode = lastDefinition.GetNode();
            _contextBuilderInputs.NewNodes.Enqueue(newNode);
            _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);
        });
    }

    private void ValidateDefinition(IHookDefinition definition)
    {
        if (definition.Validated)
            throw new Exception("Definition already validated");

        definition.Validate();
    }

}