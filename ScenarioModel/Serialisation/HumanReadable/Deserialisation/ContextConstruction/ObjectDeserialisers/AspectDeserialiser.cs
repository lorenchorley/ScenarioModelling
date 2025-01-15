using LanguageExt;
using ScenarioModelling.ContextConstruction;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers;

[ObjectLike<IDefinitionToObjectDeserialiser, Aspect>]
public class AspectDeserialiser(System System, Instanciator Instanciator, StateDeserialiser StateTransformer, RelationDeserialiser RelationTransformer) : DefinitionToObjectTransformer<Aspect, AspectReference, EntityReference>
{
    protected override Option<AspectReference> Transform(Definition def, EntityReference entity, TransformationType type)
    {
        if (def is not NamedDefinition named)
            return null;

        if (!named.Type.Value.IsEqv("Aspect"))
            return null;

        if (type == TransformationType.Property)
            throw new Exception("Aspect should not be properties of other objects");

        Aspect value = Instanciator.New<Aspect>(definition: def);

        value.Entity.SetReference(entity);
        value.Relations.TryAddReferenceRange(named.Definitions.Choose(RelationTransformer.TransformAsProperty).ToList());
        //value.AspectType = named.Definitions.Choose(d => TransformAspectType(System, d)).FirstOrDefault() ?? new AspectType(System) { Name = named.Name.Value }

        StateReference? reference = named.Definitions.Choose(StateTransformer.TransformAsProperty).FirstOrDefault();
        value.State.SetReference(reference);
        value.InitialState.SetReference(reference);

        return value.GenerateReference();
    }

    public override void BeforeIndividualValidation()
    {

    }

    public override void Validate(Aspect obj)
    {
    }
}

