using LanguageExt;
using ScenarioModel.Objects.SystemObjects.Interfaces;

namespace ScenarioModel.References.Interfaces;

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
