using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.References;

[SystemObjectLike<IReference, Constraint>]
public record ConstraintReference : IReference<Constraint>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Constraint);

    public MetaState System { get; }

    public ConstraintReference(MetaState system)
    {
        System = system;
    }
    public Option<Constraint> ResolveReference()
        => System.Constraints.Find(x => x.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
