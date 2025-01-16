using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.References.Interfaces;

namespace ScenarioModelling.References;

[SystemObjectLike<IReference, Aspect>]
public record AspectReference : IReference<Aspect>, IRelatableObjectReference, IStatefulObjectReference
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Aspect);

    public System System { get; }

    public AspectReference(System system)
    {
        System = system;
    }

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
