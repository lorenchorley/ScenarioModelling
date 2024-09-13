using LanguageExt;

namespace ScenarioModel.References;

public interface IReference
{

}

public interface IReference<T> : IReference
{
    Option<T> ResolveReference(System system);
}
