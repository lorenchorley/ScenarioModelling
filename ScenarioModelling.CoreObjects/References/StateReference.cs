using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.References;

[SystemObjectLike<IReference, State>]
public record StateReference : IReference<State>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(State);

    [JsonIgnore]
    public MetaState System { get; }

    public StateReference(MetaState system)
    {
        System = system;
    }

    public LanguageExt.Option<State> ResolveReference()
        => System.States.Find(s => s.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => Name;

}
