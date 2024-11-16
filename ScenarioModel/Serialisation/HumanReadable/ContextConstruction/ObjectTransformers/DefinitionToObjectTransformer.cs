using LanguageExt;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable;

public interface IDefinitionToObjectTransformer { }

public enum TransformationType
{
    Property,
    Object
}

public abstract class DefinitionToObjectTransformer<TVal, TRef> : IDefinitionToObjectTransformer
{
    protected abstract Option<TRef> Transform(Definition def, TransformationType type);
    public abstract void Validate(TVal obj);
    public abstract void BeforeIndividualValidation();

    public Option<TRef> TransformAsObject(Definition def)
        => Transform(def, TransformationType.Object);

    public Option<TRef> TransformAsProperty(Definition def)
        => Transform(def, TransformationType.Property);
}

public abstract class DefinitionToObjectTransformer<TVal, TRef, TArg> : IDefinitionToObjectTransformer
{
    protected abstract Option<TRef> Transform(Definition def, TArg arg, TransformationType type);
    public abstract void Validate(TVal obj);
    public abstract void BeforeIndividualValidation();

    public Option<TRef> TransformAsObject(Definition def, TArg arg)
        => Transform(def, arg, TransformationType.Object);

    public Option<TRef> TransformAsProperty(Definition def, TArg arg)
        => Transform(def, arg, TransformationType.Property);
}

