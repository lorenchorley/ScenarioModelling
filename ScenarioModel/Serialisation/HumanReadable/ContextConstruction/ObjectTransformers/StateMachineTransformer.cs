using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, StateMachine>]
public class StateMachineTransformer(System System, Instanciator Instanciator, StateTransformer StateTransformer, TransitionTransformer TransitionTransformer) : IDefinitionToObjectTransformer<StateMachine, StateMachineReference>
{
    public Option<StateMachineReference> Transform(Definition def)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.IsEqv("SM"))
        {
            return null;
        }

        StateMachine value = Instanciator.New<StateMachine>(definition: def);

        value.States.TryAddReferenceRange(unnamed.Definitions.Choose(StateTransformer.Transform));
        value.Transitions.TryAddReferenceRange(unnamed.Definitions.Choose(TransitionTransformer.Transform));

        return value.GenerateReference();
    }

    public void Validate(StateMachine obj)
    {
        //foreach (var transition in obj.Transitions)
        //{
        //    if (!obj.States.Any(s => s.IsEqv(transition.SourceState)))
        //    {
        //        State? state = System.States.Where(s => s.IsEqv(transition.SourceState)).FirstOrDefault();

        //        if (state == null)
        //        {
        //            state = new State(System) { Name = transition.SourceState.Name };
        //        }

        //        obj.States.TryAddValue(state!);
        //    }

        //    if (!obj.States.Any(s => s.IsEqv(transition.DestinationState)))
        //    {
        //        var a = System.States.ToList();
        //        State? state = System.States.Where(s => transition.DestinationState.IsEqv(s)).FirstOrDefault();

        //        if (state == null)
        //        {
        //            state = new State(System) { Name = transition.DestinationState.Name };
        //        }

        //        obj.States.TryAddValue(state!);
        //    }
        //}
    }
}

