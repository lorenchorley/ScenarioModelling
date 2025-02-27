using LanguageExt;
using Newtonsoft.Json;

namespace ScenarioModelling.CoreObjects.References.Interfaces;

public interface IReference : IIdentifiable
{
    bool IsResolvable();

    string TypeName { get; }
}

public interface IReference<T> : IReference
{
    Option<T> ResolveReference();
}

public interface IReferences<T> : IReference
{
    IEnumerable<T> ResolveReferences();
}

public abstract record ReferenceBase<T> : IReference<T>
{
    public virtual string Name { get; set; } = "";

    [JsonIgnore]
    public virtual Type Type => typeof(T);
    // [JsonDoNotIgnore]
    public string TypeName => Type.Name;

    public bool IsResolvable() => ResolveReference().IsSome;

    public abstract Option<T> ResolveReference();
}

public abstract record ReferencesBase<T> : IReferences<T>
{
    public string Name { get; set; } = "";

    [JsonIgnore]
    public Type Type => typeof(T);
    // [JsonDoNotIgnore]
    public string TypeName => Type.Name;

    public bool IsResolvable() => ResolveReferences().Any();

    public abstract IEnumerable<T> ResolveReferences();
}
