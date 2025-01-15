using LanguageExt;
using ScenarioModelling.Objects.SystemObjects.Interfaces;

namespace ScenarioModelling.References.Interfaces;

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
