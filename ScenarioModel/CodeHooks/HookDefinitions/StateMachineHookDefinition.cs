using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;

namespace ScenarioModel.CodeHooks.HookDefinitions;

public class StateMachineHookDefinition(string Name)
{
    public List<(string statefulInitial, string statefulFinal, string transitionName)> transitions = new();

    public StateMachineHookDefinition WithTransition(string statefulInitial, string statefulFinal, string transitionName)
    {
        transitions.Add((statefulInitial, statefulFinal, transitionName));
        return this;
    }

    internal StateMachine GetStateMachine()
    {
        StateMachine stateMachine = new()
        {
            Name = Name,
        };

        stateMachine.Transitions = transitions.Select(t => new Transition()
        {
            SourceState = t.statefulInitial,
            DestinationState = t.statefulFinal,
            Name = t.transitionName
        }).ToList();

        return stateMachine;
    }
}