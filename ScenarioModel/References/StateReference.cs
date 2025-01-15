using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.References;

[ObjectLike<IReference, State>]
public record StateReference : IReference<State>
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(State);

    public StateReference(System system)
    {
        _system = system;
    }

    public Option<State> ResolveReference()
        => _system.States.Find(s => s.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => Name;

}
