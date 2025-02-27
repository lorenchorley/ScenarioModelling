using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Serialisation.ContextConstruction;

namespace ScenarioModelling.CodeHooks.HookDefinitions.MetaStateObjects;

public class StateMachineHookDefinition : IObjectHookDefinition
{
    private readonly MetaState _metaState;
    private readonly Instanciator _instanciator;

    public HookExecutionMode HookExecutionMode { get; set; }
    public StateMachine StateMachine { get; private set; }
    public bool Validated { get; private set; } = false;

    // Not sure that this is necessary
    public List<(string statefulInitial, string statefulFinal, string transitionName)> transitions = new();

    public StateMachineHookDefinition(MetaState metaState, Instanciator instanciator, string name)
    {
        _metaState = metaState;
        _instanciator = instanciator;

        // Either create a new one or find an existing one in the provided system
        StateMachine = _instanciator.GetOrNew<StateMachine, StateMachineReference>(name);

    }

    public StateMachineHookDefinition WithState(string state)
    {
        StateMachine.States.TryAddReference(new StateReference(_metaState) { Name = state });

        return this;
    }

    public StateMachineHookDefinition WithTransition(string statefulInitial, string statefulFinal, string transitionName)
    {
        transitions.Add((statefulInitial, statefulFinal, transitionName));

        // Keep the tracked object up to date
        var transition = _instanciator.New<Transition>(transitionName);
        StateMachine.Transitions.TryAddReference(transition.GenerateReference());

        StateReference source = new StateReference(_metaState) { Name = statefulInitial };
        StateReference destination = new StateReference(_metaState) { Name = statefulFinal };
        transition.SourceState.SetReference(source);
        transition.DestinationState.SetReference(destination);
        StateMachine.States.TryAddReference(source);
        StateMachine.States.TryAddReference(destination);

        return this;
    }

    public void Validate()
    {
        Validated = true;
    }
}