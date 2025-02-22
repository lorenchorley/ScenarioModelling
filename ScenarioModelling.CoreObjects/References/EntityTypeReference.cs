using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;

namespace ScenarioModelling.CoreObjects.References;

[SystemObjectLike<IReference, EntityType>]
public record EntityTypeReference : IReference<EntityType>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(EntityType);

    [JsonIgnore]
    public MetaState System { get; }

    private EntityTypeReference()
    {

    }

    public EntityTypeReference(MetaState system)
    {
        System = system;
    }

    public Option<EntityType> ResolveReference()
        => System.EntityTypes.Find(x => x.IsEqv(this));

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
