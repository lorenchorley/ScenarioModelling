using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, Constraint>]
public class ConstraintTransformer(System System, Instanciator Instanciator) : IDefinitionToObjectTransformer<Constraint, ConstraintReference>
{
    public Option<ConstraintReference> Transform(Definition def)
    {
        return null;
    }

    public void Validate(Constraint obj)
    {
    }
}

