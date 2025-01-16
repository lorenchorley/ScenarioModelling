using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.References;

[SystemObjectLike<IReference, StateMachine>]
public record StateMachineReference : IReference<StateMachine>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(StateMachine);

    public System System { get; }

    public StateMachineReference(System system)
    {
        System = system;
    }

    public Option<StateMachine> ResolveReference()
        => System.StateMachines.Find(s => s.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
