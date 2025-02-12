using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;

namespace ScenarioModelling.CoreObjects.References;

[
    SystemObjectLike<IReference, Aspect>]
public record AspectReference : IReference<Aspect>, IRelatableObjectReference, IStatefulObjectReference
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(Aspect);

    public MetaState System { get; }

    public AspectReference(MetaState system)
    {
        System = system;
    }

    public Option<Aspect> ResolveReference()
        => System.Aspects.Find(x => x.IsEqv(this));

    Option<IRelatable> IReference<IRelatable>.ResolveReference()
        => ResolveReference().Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => x as IStateful);

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
