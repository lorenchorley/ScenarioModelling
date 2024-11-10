using LanguageExt;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable;

public interface IDefinitionToObjectTransformer { }
public interface IDefinitionToObjectTransformer<TVal, TRef> : IDefinitionToObjectTransformer
{
    Option<TRef> Transform(Definition def);
    void Validate(TVal obj);
}
public interface IDefinitionToObjectTransformer<TVal, TRef, TArg> : IDefinitionToObjectTransformer
{
    Option<TRef> Transform(Definition def, TArg arg);
    void Validate(TVal obj);
}

