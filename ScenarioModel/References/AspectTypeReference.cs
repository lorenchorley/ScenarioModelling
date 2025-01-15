using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.References;

//[ObjectLike<IReference, AspectType>]
public record AspectTypeReference : IReference<AspectType>, IRelatableObjectReference, IStatefulObjectReference
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(AspectType);

    public System System { get; }

    public AspectTypeReference(System system)
    {
        System = system;
    }

    public Option<AspectType> ResolveReference()
        => throw new NotImplementedException("AspectTypeReference.ResolveReference()");

    Option<IRelatable> IReference<IRelatable>.ResolveReference()
        => ResolveReference().Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => (IStateful)x);

    public bool IsResolvable() => ResolveReference().IsSome;

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
