using LanguageExt;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References.Interfaces;
using System.Text.Json.Serialization;

namespace ScenarioModel.References;

public record AspectTypeReference(System System) : IReference<AspectType>, IRelatableObjectReference, IStatefulObjectReference
{
    public string Name { get; set; } = "";
    
    [JsonIgnore]
    public Type Type => typeof(AspectType);

    public Option<AspectType> ResolveReference()
        => throw new NotImplementedException("AspectTypeReference.ResolveReference()");

    Option<IRelatable> IReference<IRelatable>.ResolveReference()
        => ResolveReference().Map(x => (IRelatable)x);

    Option<IStateful> IReference<IStateful>.ResolveReference()
        => ResolveReference().Map(x => (IStateful)x);

    public bool IsResolvable() => ResolveReference().IsSome;

    override public string ToString() => $"{Name}";

}
