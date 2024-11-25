using LanguageExt;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModel.References;

public class RelatableObjectReference : IRelatableObjectReference
{
    private readonly System _system;

    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type
        => ResolveReference().Match(
            Some: x => x.Type,
            None: () => throw new Exception($"Could not resolve relatable object {Name}")
            );

    public bool IsResolvable() => ResolveReference().IsSome;

    public RelatableObjectReference(System _system)
    {
        this._system = _system;
    }

    public Option<IRelatable> ResolveReference()
        => _system.AllRelatable
                  .Find(e => e.Name.IsEqv(Name)); // Must only search by name, because the Type property depends on resolving the reference in this type of reference

}
