using LanguageExt;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModelling.References;

[ObjectLike<IReference, StateMachine>]
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
