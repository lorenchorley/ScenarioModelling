using LanguageExt;
using ScenarioModelling.ContextConstruction;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.SystemObjectDeserialisers;

[SystemObjectLike<IDefinitionToObjectDeserialiser, StateMachine>]
public class StateMachineDeserialiser(System System, Instanciator Instanciator, StateDeserialiser StateTransformer, TransitionDeserialiser TransitionTransformer) : DefinitionToObjectDeserialiser<StateMachine, StateMachineReference>
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

        // If this is meant to be the value of a property in another object, we need to return a reference
        // Otherwise we make a full object that is stored in the system
        if (type == TransformationType.Property)
            return Instanciator.NewReference<StateMachine, StateMachineReference>(definition: def);

        StateMachine value = Instanciator.New<StateMachine>(definition: def);

        value.States.TryAddReferenceRange(unnamed.Definitions.Choose(StateTransformer.TransformAsProperty));
        value.Transitions.TryAddReferenceRange(unnamed.Definitions.Choose(TransitionTransformer.TransformAsProperty));

        return value.GenerateReference();
    }

    public override void BeforeIndividualInitialisation()
    {
        // 

        foreach (var state in System.States)
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

