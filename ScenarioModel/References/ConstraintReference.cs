using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.References;

[SystemObjectLike<IReference, Constraint>]
public record ConstraintReference : IReference<Constraint>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Constraint);

    public System System { get; }

    public ConstraintReference(System system)
    {
        System = system;
    }
    public Option<Constraint> ResolveReference()
        => System.Constraints.Find(x => x.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
