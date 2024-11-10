using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, Aspect>]
public class AspectTransformer(System System, Instanciator Instanciator, StateTransformer StateTransformer, RelationTransformer RelationTransformer) : IDefinitionToObjectTransformer<Aspect, AspectReference, EntityReference>
{
    public Option<AspectReference> Transform(Definition def, EntityReference entity)
    {
        if (def is not NamedDefinition named)
        {
            return null;
        }

        if (!named.Type.Value.IsEqv("Aspect"))
        {
            return null;
        }

        Aspect value = Instanciator.New<Aspect>(definition: def);

        value.Entity.SetReference(entity);
        value.Relations.TryAddReferenceRange(named.Definitions.Choose(RelationTransformer.Transform).ToList());
        //value.AspectType = named.Definitions.Choose(d => TransformAspectType(System, d)).FirstOrDefault() ?? new AspectType(System) { Name = named.Name.Value }

        value.State.SetReference(named.Definitions.Choose(StateTransformer.Transform).FirstOrDefault());


        return value.GenerateReference();
    }

    public void Validate(Aspect obj)
    {
    }
}

