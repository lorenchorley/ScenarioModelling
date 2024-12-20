﻿using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers;

[ObjectLike<IDefinitionToObjectDeserialiser, Entity>]
public class EntityDeserialiser(System System, Instanciator Instanciator, StateDeserialiser StateTransformer, AspectDeserialiser AspectTransformer, EntityTypeDeserialiser EntityTypeTransformer, RelationDeserialiser RelationTransformer) : DefinitionToObjectDeserialiser<Entity, EntityReference>
{
    protected override Option<EntityReference> Transform(Definition def, TransformationType type)
    {
        if (def is not UnnamedDefinition unnamed)
            return null;

        if (!unnamed.Type.Value.IsEqv("Entity"))
            return null;

        if (type == TransformationType.Property)
            throw new Exception("Entity should not be properties of other objects");

        var (relations, remaining1) = unnamed.Definitions.PartitionByChoose(RelationTransformer.TransformAsProperty);
        var (stateReferences, remaining2) = remaining1.PartitionByChoose(StateTransformer.TransformAsProperty);


        Entity value = Instanciator.New<Entity>(definition: def);

        EntityTypeReference typeReference =
            unnamed.Definitions
                   .Choose(EntityTypeTransformer.TransformAsProperty)
                   .FirstOrDefault()
                   ?? Instanciator.NewReference<EntityType, EntityTypeReference>();

        value.EntityType.SetReference(typeReference);
        value.Relations.TryAddReferenceRange(relations);

        value.Aspects.TryAddReferenceRange(unnamed.Definitions.Choose(d => AspectTransformer.TransformAsObject(d, value.GenerateReference())));
        value.State.SetReference(stateReferences.FirstOrDefault());
        value.CharacterStyle = unnamed.Definitions.Choose(TransformCharacterStyle).FirstOrDefault() ?? "";

        if (stateReferences.Count() > 1)
        {
            throw new Exception($"More than one state was set on entity {value.Name ?? "<unnamed>"} of type {typeReference?.Name ?? "<unnnamed>"} : {stateReferences.Select(s => s.Name).CommaSeparatedList()}");
        }

        return value.GenerateReference();
    }

    private Option<string> TransformCharacterStyle(Definition definition)
    {
        if (definition is not NamedDefinition named)
        {
            return null;
        }

        if (named.Type.IsEqv("CharacterStyle"))
        {
            return named.Name.Value;
        }

        return null;
    }

    public override void BeforeIndividualInitialisation()
    {

    }

    public override void Initialise(Entity obj)
    {
    }
}

