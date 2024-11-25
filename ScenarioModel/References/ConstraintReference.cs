using LanguageExt;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModel.References;

public record ConstraintReference(System System) : IReference<Constraint>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Constraint);

    public Option<Constraint> ResolveReference()
        => System.Constraints.Find(x => x.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
