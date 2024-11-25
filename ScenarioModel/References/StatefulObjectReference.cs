using LanguageExt;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModel.References;

public class StatefulObjectReference : IStatefulObjectReference
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type
        => ResolveReference().Match(
            Some: x => x.Type,
            None: () => throw new Exception($"Could not resolve stateful object {Name}")
            );

    public StatefulObjectReference(System system)
    {
        _system = system;
    }

    public Option<IStateful> ResolveReference()
        => _system.AllStateful
                  .Find(e => e.Name.IsEqv(Name)); // Must only search by name, because the Type property depends on resolving the reference in this type of reference

    public bool IsResolvable() => ResolveReference().IsSome;

}
