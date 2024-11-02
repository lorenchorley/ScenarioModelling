namespace ScenarioModel.CodeHooks.HookDefinitions;

public class StateMachineHookDefinition(string Name)
{
    public List<(string statefulInitial, string statefulFinal, string transitionName)> transitions = new();

    public StateMachineHookDefinition WithTransition(string statefulInitial, string statefulFinal, string transitionName)
    {
        transitions.Add((statefulInitial, statefulFinal, transitionName));
        return this;
    }
}