using LanguageExt;
using ScenarioModel.Objects.SystemObjects.States;

namespace ScenarioModel.References;

public record StateMachineReference : IReference<StateMachine>
{
    public string StateMachineName { get; set; } = "";

    public Option<StateMachine> ResolveReference(System system)
    {
        return system.StateMachines.Find(s => s.Name.IsEqv(StateMachineName));
    }

    override public string ToString() => $"{StateMachineName}";
}
