using LanguageExt;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModel.References;

public record StateMachineReference(System System) : IReference<StateMachine>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(StateMachine);

    public Option<StateMachine> ResolveReference()
        => System.StateMachines.Find(s => s.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
