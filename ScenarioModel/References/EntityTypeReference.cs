using LanguageExt;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModel.References;

public record EntityTypeReference(System System) : IReference<EntityType>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(EntityType);

    public Option<EntityType> ResolveReference()
        => System.EntityTypes.Find(x => x.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
