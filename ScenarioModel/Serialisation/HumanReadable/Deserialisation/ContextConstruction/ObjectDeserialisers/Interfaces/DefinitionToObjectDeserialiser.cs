using LanguageExt;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers.Interfaces;

public abstract class DefinitionToObjectDeserialiser<TVal, TRef> : IDefinitionToObjectDeserialiser
{
    protected abstract Option<TRef> Transform(Definition def, TransformationType type);
    public abstract void Initialise(TVal obj);
    public abstract void BeforeIndividualInitialisation();

    public Option<TRef> TransformAsObject(Definition def)
        => Transform(def, TransformationType.Object);

    public Option<TRef> TransformAsProperty(Definition def)
        => Transform(def, TransformationType.Property);
}

public abstract class DefinitionToObjectTransformer<TVal, TRef, TArg> : IDefinitionToObjectDeserialiser
{
    protected abstract Option<TRef> Transform(Definition def, TArg arg, TransformationType type);
    public abstract void Validate(TVal obj);
    public abstract void BeforeIndividualValidation();

    public Option<TRef> TransformAsObject(Definition def, TArg arg)
        => Transform(def, arg, TransformationType.Object);

    public Option<TRef> TransformAsProperty(Definition def, TArg arg)
        => Transform(def, arg, TransformationType.Property);
}

