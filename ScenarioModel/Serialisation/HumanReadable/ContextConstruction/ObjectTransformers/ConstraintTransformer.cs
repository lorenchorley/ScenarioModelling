using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, Constraint>]
public class ConstraintTransformer(System System, Instanciator Instanciator) : DefinitionToObjectTransformer<Constraint, ConstraintReference>
{
    protected override Option<ConstraintReference> Transform(Definition def, TransformationType type)
    {
        return null;
    }

    public override void Validate(Constraint obj)
    {
    }
}

