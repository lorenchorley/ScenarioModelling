using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;

namespace ScenarioModelling.CoreObjects.References.GeneralisedReferences;

public class StatefulObjectReference : IStatefulObjectReference
{
    private readonly MetaState _metaState;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type
        => ResolveReference().Match(
            Some: x => x.Type,
            None: () => throw new Exception($"Could not resolve stateful object {Name}")
            );

    // [JsonDoNotIgnore]
    public string TypeName => Type.Name;

    public StatefulObjectReference(MetaState metaState)
    {
        _metaState = metaState;
    }

    public Option<IStateful> ResolveReference()
        => _metaState.AllStateful
                     .Find(e => IsStatefulObjectEqv(e, Name)); // Must only search by name, because the Type property depends on resolving the reference in this type of reference

    private static bool IsStatefulObjectEqv(IStateful other, string name)
    {
        // TODO if has .State on the end, remove it and compare as normal ?

        if (other is Aspect aspect)
        {
            string aspectIdentifier = $"{aspect.Entity.Name}.{aspect.Name}";
            return aspectIdentifier.IsEqv(name);
        }

        return other.Name.IsEqv(name);
    }

    public bool IsResolvable() => ResolveReference().IsSome;

    public bool IsEqv(IStatefulObjectReference other)
    {
        if (other is not StatefulObjectReference otherReference)
            return false;

        if (!otherReference.Name.IsEqv(Name))
            return false;

        return true;
    }
}
