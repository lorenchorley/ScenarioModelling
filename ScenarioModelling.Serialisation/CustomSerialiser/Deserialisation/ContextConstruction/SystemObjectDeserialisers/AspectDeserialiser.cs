using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers;

[MetaStateObjectLike<IDefinitionToObjectDeserialiser, Aspect>]
public class AspectDeserialiser(MetaState MetaState, Instanciator Instanciator, StateDeserialiser StateTransformer, RelationDeserialiser RelationTransformer) : DefinitionToObjectTransformer<Aspect, AspectReference, EntityReference>
{
    protected override Option<AspectReference> Transform(Definition def, EntityReference entity, TransformationType type)
    {
        if (def is not NamedDefinition named)
            return null;

        if (!named.Type.Value.IsEqv("Aspect"))
            return null;

        if (type == TransformationType.Property)
            throw new Exception("Aspect should not be properties of other objects");


        def.HasBeenTransformed = true;

        Aspect value = Instanciator.NewUnregistered<Aspect>(definition: def);

        if (MetaState.Aspects.Any(e => e.Name == value.Name))
        {
            // If an object of the same type with the same name already exists,
            // we remove this one and but return the object as if it we've transformed so that it doesn't get signaled as not transformed
            return value.GenerateReference();
        }

        value.Entity.SetReference(entity);
        value.Relations.TryAddReferenceRange(named.Definitions.Choose(RelationTransformer.TransformAsProperty));
        //value.AspectType = named.Definitions.Choose(d => TransformAspectType(System, d)).FirstOrDefault() ?? new AspectType(System) { Name = named.Name.Value }

        StateReference? reference = named.Definitions.Choose(StateTransformer.TransformAsProperty).FirstOrDefault();
        value.State.SetReference(reference);
        value.InitialState.SetReference(reference);

        Instanciator.RegisterWithMetaState(value);
        return value.GenerateReference();
    }

    public override void BeforeIndividualValidation()
    {

    }

    public override void Validate(Aspect obj)
    {
    }
}

