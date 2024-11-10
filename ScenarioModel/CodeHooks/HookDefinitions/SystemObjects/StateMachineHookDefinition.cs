using ScenarioModel.ContextConstruction;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions.SystemObjects;

public class StateMachineHookDefinition : IObjectHookDefinition
{
    private readonly System _system;
    private readonly Instanciator _instanciator;

    public HookExecutionMode HookExecutionMode { get; set; }
    public StateMachine StateMachine { get; private set; }

    // Not sure that this is necessary
    public List<(string statefulInitial, string statefulFinal, string transitionName)> transitions = new();
    
    public StateMachineHookDefinition(System system, Instanciator instanciator, string name)
    {
        _system = system;
        _instanciator = instanciator;
    
        // Either create a new one or find an existing one in the provided system
        StateMachine = _instanciator.GetOrNew<StateMachine, StateMachineReference>(name);
        //StateMachine = new StateMachineReference(_system) { Name = name }
        //    .ResolveReference()
        //    .Match(
        //        Some: e => e,
        //        None: () => _instanciator.New<StateMachine>(name: name));
    }

    public StateMachineHookDefinition WithState(string state)
    {
        StateMachine.States.TryAddReference(new StateReference(_system) { Name = state });

        return this;
    }

    public StateMachineHookDefinition WithTransition(string statefulInitial, string statefulFinal, string transitionName)
    {
        transitions.Add((statefulInitial, statefulFinal, transitionName));

        // Keep the tracked object up to date
        var transition = _instanciator.New<Transition>(transitionName);
        StateMachine.Transitions.TryAddReference(transition.GenerateReference());

        StateReference source = new StateReference(_system) { Name = statefulInitial };
        StateReference destination = new StateReference(_system) { Name = statefulFinal };
        transition.SourceState.SetReference(source);
        transition.DestinationState.SetReference(destination);
        StateMachine.States.TryAddReference(source);
        StateMachine.States.TryAddReference(destination);

        return this;
    }
}