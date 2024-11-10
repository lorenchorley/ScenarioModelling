using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

using Relation = ScenarioModel.Objects.SystemObjects.Relation;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, Relation>]
public class RelationTransformer(System System, Instanciator Instanciator) : IDefinitionToObjectTransformer<Relation, RelationReference>
{
    public Option<RelationReference> Transform(Definition def)
    {
        if (def is not UnnamedLinkDefinition unnamed)
        {
            return null;
        }

        Relation value = Instanciator.New<Relation>(definition: def);

        value.LeftEntity = new RelatableObjectReference(System)
        {
            Name = unnamed.Source.Value
        };

        value.RightEntity = new RelatableObjectReference(System)
        {
            Name = unnamed.Destination.Value
        };
        //value.LeftEntity = new CompositeValueObjectReference(System)
        //{
        //    Identifier = unnamed.Source
        //};

        //value.RightEntity = new CompositeValueObjectReference(System)
        //{
        //    Identifier = unnamed.Destination
        //};
        // TODO State ?

        return value.GenerateReference();
    }

    public void Validate(Relation obj)
    {
    }
}

