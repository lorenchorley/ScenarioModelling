﻿using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, Entity>]
public class EntityTransformer(System System, Instanciator Instanciator, StateTransformer StateTransformer, AspectTransformer AspectTransformer, EntityTypeTransformer EntityTypeTransformer, RelationTransformer RelationTransformer) : IDefinitionToObjectTransformer<Entity, EntityReference>
{
    public Option<EntityReference> Transform(Definition def)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.IsEqv("Entity"))
        {
            return null;
        }

        var (relations, remaining1) = unnamed.Definitions.PartitionByChoose(RelationTransformer.Transform);
        var (stateReferences, remaining2) = remaining1.PartitionByChoose(StateTransformer.Transform);


        Entity value = Instanciator.New<Entity>(definition: def);

        EntityTypeReference type =
            unnamed.Definitions
                   .Choose(EntityTypeTransformer.Transform)
                   .FirstOrDefault()
                   ?? Instanciator.NewReference<EntityTypeReference>();

        value.EntityType.SetReference(type);
        value.Relations.TryAddReferenceRange(relations);

        value.Aspects.TryAddReferenceRange(unnamed.Definitions.Choose(d => AspectTransformer.Transform(d, value.GenerateReference())));
        value.State.SetReference(stateReferences.FirstOrDefault());
        value.CharacterStyle = unnamed.Definitions.Choose(TransformCharacterStyle).FirstOrDefault() ?? "";

        if (stateReferences.Count() > 1)
        {
            throw new Exception($"More than one state was set on entity {value.Name ?? "<unnamed>"} of type {type?.Name ?? "<unnnamed>"} : {stateReferences.Select(s => s.Name).CommaSeparatedList()}");
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

    public void Validate(Entity obj)
    {
    }
}

