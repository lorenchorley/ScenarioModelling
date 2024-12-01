using ScenarioModel.CodeHooks.HookDefinitions;
using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;

namespace ScenarioModel.CodeHooks;

public abstract class ScenarioHookOrchestrator
{
    public Context Context { get; }

    protected ScenarioHookDefinition? _scenarioDefintion;
    protected readonly Stack<DefinitionScope> _scopeStack = new();
    protected readonly HookContextBuilderInputs _contextBuilderInputs;
    protected readonly ProgressiveHookBasedContextBuilder _contextBuilder;

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
        return _scenarioDefintion?.GetScenario() ?? throw new ArgumentNullException();
    }

    public virtual DialogHookDefinition DeclareDialog(string text)
    {
        DialogHookDefinition nodeDef = new(text);
        CurrentScope.AddNodeDefintion(nodeDef);

        _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

        return nodeDef;
    }

    public virtual DialogHookDefinition DeclareDialog(string character, string text)
    {
        DialogHookDefinition nodeDef = new DialogHookDefinition(text).SetCharacter(character);
        CurrentScope.AddNodeDefintion(nodeDef);

        _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

        return nodeDef;
    }

    public virtual ChooseHookDefinition DeclareChoose(params ChoiceList choices)
    {
        ChooseHookDefinition nodeDef = new();
        nodeDef.Node.Choices.AddRange(choices);
        CurrentScope.AddNodeDefintion(nodeDef);

        _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

        return nodeDef;
    }

    public virtual TransitionHookDefinition DeclareTransition(string statefulObjectName, string transition)
    {
        TransitionHookDefinition nodeDef = new(Context.System, statefulObjectName, transition);
        CurrentScope.AddNodeDefintion(nodeDef);

        _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

        return nodeDef;
    }

    public virtual IfHookDefinition DeclareIfBranch(string condition)
    {
        IfHookDefinition nodeDef = new(condition);
        CurrentScope.AddNodeDefintion(nodeDef);

        _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

        return nodeDef;
    }

    public virtual JumpHookDefinition DeclareJump(string target)
    {
        if (string.IsNullOrEmpty(target))
            throw new ArgumentNullException(nameof(target));

        JumpHookDefinition nodeDef = new(target);
        CurrentScope.AddNodeDefintion(nodeDef);

        _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

        return nodeDef;
    }

    public virtual WhileHookDefinition DeclareWhileBranch(string condition)
    {
        WhileHookDefinition nodeDef = new(condition, CurrentScope);

        _contextBuilderInputs.NewNodes.Enqueue(nodeDef.GetNode());
        _contextBuilder.BuildContextFromInputs(_contextBuilderInputs);

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