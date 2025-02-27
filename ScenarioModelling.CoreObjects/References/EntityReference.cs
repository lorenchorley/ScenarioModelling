using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

namespace ScenarioModelling.CoreObjects.References;

[MetaStateObjectLike<IReference, Entity>]
public record EntityReference : ReferenceBase<Entity>, IRelatableObjectReference, IStatefulObjectReference
{
    [JsonIgnore]
    public MetaState MetaState { get; }

    public EntityReference(MetaState system)
    {
        MetaState = system;
    }

    public override Option<Entity> ResolveReference()
        => MetaState.Entities.Find(x => x.IsEqv(this));

    Option<IRelatable> IReference<IRelatable>.ResolveReference()
        => ResolveReference().Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => (IStateful)x);

    override public string ToString() => $"{Name}";


    public bool IsEqv(IStatefulObjectReference other)
    {
        if (other is not EntityReference otherReference)
            return false;

        if (!otherReference.Name.IsEqv(Name))
            return false;

        return true;
    }
}
