using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions.SystemObjects;

public class StateMachineHookDefinition : IObjectHookDefinition
{
    public HookExecutionMode HookExecutionMode { get; set; }
    public StateMachine StateMachine { get; private set; }

    public List<(string statefulInitial, string statefulFinal, string transitionName)> transitions = new();

    public StateMachineHookDefinition(System system, string name)
    {
        // Either create a new one or find an existing one in the provided system
        StateMachine = new StateMachineReference() { StateMachineName = name }
            .ResolveReference(system)
            .Match(
                Some: e => e,
                None: () => New(system, name));
    }

    private StateMachine New(System system, string name)
        => new StateMachine(system)
        {
            Name = name
        };

    public StateMachineHookDefinition WithState(string state)
    {
        TryAddState(state);

        return this;
    }

    public StateMachineHookDefinition WithTransition(string statefulInitial, string statefulFinal, string transitionName)
    {
        transitions.Add((statefulInitial, statefulFinal, transitionName));

        // Keep the tracked object up to date
        StateMachine.Transitions.Add(new Transition()
        {
            SourceState = statefulInitial,
            DestinationState = statefulFinal,
            Name = transitionName
        });

        TryAddState(statefulInitial);
        TryAddState(statefulFinal);

        return this;
    }

    private void TryAddState(string statefulFinal)
    {
        if (!StateMachine.States.Any(s => s.Name.IsEqv(statefulFinal)))
        {
            StateMachine.States.Add(new State()
            {
                Name = statefulFinal
            });
        }
    }
}