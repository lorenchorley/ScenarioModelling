using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.References;

[SystemObjectLike<IReference, EntityType>]
public record EntityTypeReference : IReference<EntityType>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(EntityType);

    public System System { get; }

    public EntityTypeReference(System system)
    {
        System = system;
    }

    public Option<EntityType> ResolveReference()
        => System.EntityTypes.Find(x => x.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
