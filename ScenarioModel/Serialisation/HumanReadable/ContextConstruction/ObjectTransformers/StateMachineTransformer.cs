using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, StateMachine>]
public class StateMachineTransformer(System System, Instanciator Instanciator, StateTransformer StateTransformer, TransitionTransformer TransitionTransformer) : DefinitionToObjectTransformer<StateMachine, StateMachineReference>
{
    protected override Option<StateMachineReference> Transform(Definition def, TransformationType type)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.IsEqv("SM"))
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

    public override void BeforeIndividualValidation()
    {
        foreach (var state in System.States)
        {
            if (!System.StateMachines.Any(SM => SM.States.Any(s => s.IsEqv(state))))
            {
                Instanciator.New<StateMachine>().States.TryAddValue(state);
            }
        }
    }

    public override void Validate(StateMachine obj)
    {
        //foreach (var item in System.States.Where(s => s.StateMachine.IsEqv(obj)))
        //{
        //    obj.States.TryAddValue(item);
        //}
    }

}

