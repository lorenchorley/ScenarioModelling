using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

namespace ScenarioModelling.CoreObjects.References;

//[ObjectLike<IReference, AspectType>]
public record AspectTypeReference : ReferenceBase<AspectType>, IRelatableObjectReference, IStatefulObjectReference
{
    [JsonIgnore]
    public MetaState MetaState { get; }

    public AspectTypeReference(MetaState system)
    {
        MetaState = system;
    }

    public override Option<AspectType> ResolveReference()
        => throw new NotImplementedException("AspectTypeReference.ResolveReference()");

    Option<IRelatable> IReference<IRelatable>.ResolveReference()
        => ResolveReference().Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => (IStateful)x);

    override public string ToString() => $"{Name}";

    public bool IsEqv(IStatefulObjectReference other)
    {
        if (other is not AspectTypeReference otherReference)
            return false;

        if (!otherReference.Name.IsEqv(Name))
            return false;

        return true;
    }

}
