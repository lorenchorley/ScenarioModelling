using ScenarioModel.CodeHooks.HookDefinitions;
using ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;

namespace ScenarioModel.CodeHooks;

public abstract class ScenarioHookOrchestrator
{
    protected ScenarioHookDefinition? _scenarioDefintion;

    protected readonly Stack<DefinitionScope> _scopeStack = new();

    public Context Context { get; }

    protected DefinitionScope CurrentScope
    {
        get => _scopeStack.Peek();
    }

    protected Scenario Scenario
    {
        get => _scenarioDefintion?.GetScenario()
                                 ?? throw new ArgumentNullException();
    }

    protected System System
    {
        get => Scenario.System;
    }

    protected ScenarioHookOrchestrator(Context context)
    {
        NodeExhaustiveness.AssertExhaustivelyImplemented<INodeHookDefinition>();

        Context = context;
    }

    public virtual ScenarioHookDefinition? DeclareScenarioStart(string name)
    {
        _scenarioDefintion = new ScenarioHookDefinition(name, Context);

        _scopeStack.Push(new DefinitionScope()
        {
            SubGraph = Scenario.Graph.PrimarySubGraph
        });

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
        return nodeDef;
    }

    public virtual DialogHookDefinition DeclareDialog(string character, string text)
    {
        DialogHookDefinition nodeDef = new DialogHookDefinition(text).SetCharacter(character);
        CurrentScope.AddNodeDefintion(nodeDef);
        return nodeDef;
    }

    public virtual ChooseHookDefinition DeclareChoose(params ChoiceList choices)
    {
        ChooseHookDefinition nodeDef = new();
        nodeDef.Choices.AddRange(choices);

        CurrentScope.AddNodeDefintion(nodeDef);
        return nodeDef;
    }

    public virtual StateTransitionHookDefinition DeclareTransition(string statefulObjectName, string transition)
    {
        StateTransitionHookDefinition nodeDef = new(statefulObjectName, transition);
        CurrentScope.AddNodeDefintion(nodeDef);
        return nodeDef;
    }

    public virtual IfHookDefinition DeclareIfBranch(string condition)
    {
        IfHookDefinition nodeDef = new(condition);
        CurrentScope.AddNodeDefintion(nodeDef);
        return nodeDef;
    }

    public virtual JumpHookDefinition DeclareJump(string target)
    {
        JumpHookDefinition nodeDef = new(target);
        CurrentScope.AddNodeDefintion(nodeDef);
        return nodeDef;
    }

    public virtual WhileHookDefinition DeclareWhileBranch(string condition)
    {
        WhileHookDefinition nodeDef = new(condition, CurrentScope);
        return nodeDef;
    }

    public void DefineSystem(Action<SystemHookDefinition> configure)
    {
        SystemHookDefinition systemHookDefinition = new(Context);
        configure(systemHookDefinition);
        systemHookDefinition.Initialise();
    }
}