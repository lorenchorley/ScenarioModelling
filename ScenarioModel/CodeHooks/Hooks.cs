using ScenarioModel.CodeHooks.HookDefinitions;
using ScenarioModel.Exhaustiveness;

namespace ScenarioModel.CodeHooks;

public abstract class Hooks
{
    protected ScenarioHookDefinition? _scenarioDefintion;
    protected List<EntityHookDefinition> _entityDefintions = new();
    protected List<StateMachineHookDefinition> _stateMachineDefintions = new();

    protected readonly Stack<DefinitionScope> _scopeStack = new();

    protected DefinitionScope CurrentScope
    {
        get => _scopeStack.Peek();
    }

    protected Hooks()
    {
        NodeExhaustiveness.AssertExhaustivelyImplemented<INodeHookDefinition>();

        _scopeStack.Push(new DefinitionScope());
    }

    public abstract ScenarioHookDefinition? DeclareScenarioStart(string name);

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

    public virtual ChooseHookDefinition DeclareChoose(params string[] choices)
    {
        ChooseHookDefinition nodeDef = new(choices);
        CurrentScope.AddNodeDefintion(nodeDef);
        return nodeDef;
    }

    public virtual StateTransitionHookDefinition DeclareTransition(string actor, string transition)
    {
        StateTransitionHookDefinition nodeDef = new(actor);
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

    public EntityHookDefinition DefineEntity(string name)
    {
        EntityHookDefinition nodeDef = new(name);
        _entityDefintions.Add(nodeDef);
        return nodeDef;
    }

    public StateMachineHookDefinition DefineStateMachine(string name)
    {
        StateMachineHookDefinition nodeDef = new(name);
        _stateMachineDefintions.Add(nodeDef);
        return nodeDef;
    }
}