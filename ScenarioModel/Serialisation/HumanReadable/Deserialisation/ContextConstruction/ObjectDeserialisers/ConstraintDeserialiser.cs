using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers;

[ObjectLike<IDefinitionToObjectDeserialiser, Constraint>]
public class ConstraintDeserialiser(System System, Instanciator Instanciator) : DefinitionToObjectDeserialiser<Constraint, ConstraintReference>
{
    protected override Option<ConstraintReference> Transform(Definition def, TransformationType type)
    {
        return null;
    }

    public override void BeforeIndividualInitialisation()
    {

    }

    public override void Initialise(Constraint obj)
    {
    }
}

