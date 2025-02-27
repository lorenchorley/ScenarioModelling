using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;

namespace ScenarioModelling.CoreObjects.References;

[MetaStateObjectLike<IReference, Aspect>]
public record AspectReference : ReferenceBase<Aspect>, IRelatableObjectReference, IStatefulObjectReference
{
    [JsonIgnore]
    public MetaState MetaState { get; }

    public AspectReference(MetaState metaState)
    {
        MetaState = metaState;
    }

    public override Option<Aspect> ResolveReference()
        => MetaState.Aspects.Find(x => x.IsEqv(this));

    Option<IRelatable> IReference<IRelatable>.ResolveReference()
        => ResolveReference().Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => x as IStateful);

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
