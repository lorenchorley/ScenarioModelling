using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;

namespace ScenarioModelling.CoreObjects.References.GeneralisedReferences;

public class StatefulObjectReference : IStatefulObjectReference
{
    private readonly MetaState _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type
        => ResolveReference().Match(
            Some: x => x.Type,
            None: () => throw new Exception($"Could not resolve stateful object {Name}")
            );

    public StatefulObjectReference(MetaState system)
    {
        _system = system;
    }

    public Option<IStateful> ResolveReference()
        => _system.AllStateful
                  .Find(e => e.Name.IsEqv(Name)); // Must only search by name, because the Type property depends on resolving the reference in this type of reference

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
