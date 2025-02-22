using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.References;

[SystemObjectLike<IReference, StateMachine>]
public record StateMachineReference : IReference<StateMachine>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(StateMachine);

    [JsonIgnore]
    public MetaState System { get; }

    public StateMachineReference(MetaState system)
    {
        System = system;
    }

    public Option<StateMachine> ResolveReference()
        => System.StateMachines.Find(s => s.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
