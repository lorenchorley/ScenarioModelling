using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.References;

[SystemObjectLike<IReference, State>]
public record StateReference : IReference<State>
{
    private readonly MetaState _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(State);

    public StateReference(MetaState system)
    {
        _system = system;
    }

    public LanguageExt.Option<State> ResolveReference()
        => _system.States.Find(s => s.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => Name;

}
