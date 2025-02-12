using LanguageExt;

namespace ScenarioModelling.CoreObjects.References.Interfaces;

public interface IReference : IIdentifiable
{
    bool IsResolvable();
}

public interface IReference<T> : IReference
{
    Option<T> ResolveReference();
}

public interface IReferences<T> : IReference
{
    IEnumerable<T> ResolveReferences();
}
