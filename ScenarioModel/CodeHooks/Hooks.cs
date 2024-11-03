using ScenarioModel.CodeHooks.HookDefinitions;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;

namespace ScenarioModel.CodeHooks;

public abstract class Hooks
{
    protected ScenarioHookDefinition? _scenarioDefintion;
    protected List<EntityHookDefinition> _entityDefintions = new();
    protected List<StateMachineHookDefinition> _stateMachineDefintions = new();

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

    protected Hooks(Context context)
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
        WhileHookDefinition nodeDef = new(condition);
        CurrentScope.AddNodeDefintion(nodeDef);
        return nodeDef;
    }

    public EntityHookDefinition DefineEntity(string name)
    {
        EntityHookDefinition nodeDef = new(System, name);
        _entityDefintions.Add(nodeDef);

        Scenario.System.Entities.Add(nodeDef.GetEntity());

        return nodeDef;
    }

    public StateMachineHookDefinition DefineStateMachine(string name)
    {
        StateMachineHookDefinition nodeDef = new(name);
        _stateMachineDefintions.Add(nodeDef);

        Scenario.System.StateMachines.Add(nodeDef.GetStateMachine());

        return nodeDef;
    }
}