using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.References.Interfaces;

namespace ScenarioModelling.CoreObjects.References;

[MetaStateObjectLike<IReference, EntityType>]
public record EntityTypeReference : ReferenceBase<EntityType>
{
    [JsonIgnore]
    public MetaState MetaState { get; }

    private EntityTypeReference()
    {

    }

    public EntityTypeReference(MetaState system)
    {
        MetaState = system;
    }

    public override Option<EntityType> ResolveReference()
        => MetaState.EntityTypes.Find(x => x.IsEqv(this));

    override public string ToString() => $"{Name}";

}
