using LanguageExt;
using Newtonsoft.Json;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;

namespace ScenarioModelling.CoreObjects.References.GeneralisedReferences;

public class RelatableObjectReference : IRelatableObjectReference
{
    private readonly MetaState _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type
        => ResolveReference().Match(
            Some: x => x.Type,
            None: () => throw new Exception($"Could not resolve relatable object {Name}")
            );

    public bool IsResolvable() => ResolveReference().IsSome;

    public RelatableObjectReference(MetaState _system)
    {
        this._system = _system;
    }

    public Option<IRelatable> ResolveReference()
        => _system.AllRelatable
                  .Find(e => e.Name.IsEqv(Name)); // Must only search by name, because the Type property depends on resolving the reference in this type of reference

}
