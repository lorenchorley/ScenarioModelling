﻿using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers;

[MetaStateObjectLike<IDefinitionToObjectDeserialiser, StateMachine>]
public class StateMachineDeserialiser(MetaState MetaState, Instanciator Instanciator, StateDeserialiser StateTransformer, TransitionDeserialiser TransitionTransformer) : DefinitionToObjectDeserialiser<StateMachine, StateMachineReference>
{
    protected override Option<StateMachineReference> Transform(Definition def, TransformationType type)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.IsEqv("StateMachine"))
        {
            return null;
        }

        def.HasBeenTransformed = true;

        // If this is meant to be the value of a property in another object, we need to return a reference
        // Otherwise we make a full object that is stored in the system
        if (type == TransformationType.Property)
            return Instanciator.NewReference<StateMachine, StateMachineReference>(definition: def);


        StateMachine value = Instanciator.NewUnregistered<StateMachine>(definition: def);

        if (MetaState.StateMachines.Any(e => e.Name == value.Name))
        {
            // If an object of the same type with the same name already exists,
            // we remove this one and but return the object as if it we've transformed so that it doesn't get signaled as not transformed
            return value.GenerateReference();
        }

        value.States.TryAddReferenceRange(unnamed.Definitions.Choose(StateTransformer.TransformAsProperty));
        value.Transitions.TryAddReferenceRange(unnamed.Definitions.Choose(TransitionTransformer.TransformAsProperty));

        Instanciator.RegisterWithMetaState(value);
        return value.GenerateReference();
    }

    public override void BeforeIndividualInitialisation()
    {
        // 

        foreach (var state in MetaState.States)
        {

        }
    }

    public override void Initialise(StateMachine obj)
    {
        //foreach (var item in System.States.Where(s => s.StateMachine.IsEqv(obj)))
        //{
        //    obj.States.TryAddValue(item);
        //}
    }

}

