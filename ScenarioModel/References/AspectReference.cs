using LanguageExt;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModelling.References;

[ObjectLike<IReference, Aspect>]
public record AspectReference(System System) : IReference<Aspect>, IRelatableObjectReference, IStatefulObjectReference
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Aspect);

    public Option<Aspect> ResolveReference()
        => System.Aspects.Find(x => x.IsEqv(this));

    Option<IRelatable> IReference<IRelatable>.ResolveReference()
        => ResolveReference().Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => (IStateful)x);

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

    public bool IsEqv(IStatefulObjectReference other)
    {
        if (other is not AspectReference otherReference)
            return false;

        if (!otherReference.Name.IsEqv(Name))
            return false;

        return true;
    }
}
