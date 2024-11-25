using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.ObjectDeserialisers;

[ObjectLike<IDefinitionToObjectDeserialiser, Transition>]
public class TransitionDeserialiser(System System, Instanciator Instanciator) : DefinitionToObjectDeserialiser<Transition, TransitionReference>
{
    /// <summary>
    /// Should return a reference. The instance should be recorded to the system by the class itself
    /// </summary>
    /// <param name="def"></param>
    /// <returns></returns>
    protected override Option<TransitionReference> Transform(Definition def, TransformationType type)
    {
        if (def is not UnnamedLinkDefinition unnamed)
            return null;

        //if (!unnamed.Type.Value.IsEqv("Transition"))
        //    return null;

        if (type != TransformationType.Property)
            throw new Exception("A transition must always be the propert of another object");

        Transition value = Instanciator.New<Transition>(definition: def);
        value.SourceState.SetReference(Instanciator.NewReference<State, StateReference>(name: unnamed.Source.Value));
        value.DestinationState.SetReference(Instanciator.NewReference<State, StateReference>(name: unnamed.Destination.Value));

        return value.GenerateReference();
    }

    public override void BeforeIndividualInitialisation()
    {
        // TODO For each state on each transition, we need to associate them with their state machine
        foreach (var transition in System.Transitions)
        {
            StateMachine? sm =
                System.StateMachines
                      .Where(sm => sm.Transitions.Any(t => t.IsEqv(transition)))
                      .FirstOrDefault();

            if (sm == null)
                throw new Exception($"Transition does not have an associated state machine : {transition}");

            // TODO set states state machine
            var sourceReference = transition.SourceState.GetOrGenerateReference();
            var destinationReference = transition.DestinationState.GetOrGenerateReference();

            if (sourceReference == null)
                throw new Exception($"Transition does not have a source state : {transition}");

            if (destinationReference == null)
                throw new Exception($"Transition does not have a destination state : {transition}");

            sm.States.TryAddReference(sourceReference);
            sm.States.TryAddReference(destinationReference);
        }
    }

    public override void Initialise(Transition obj)
    {
        //foreach (var state in type.States)
        //{
        //    //state.StateMachine = type;

        //    var transitionsForState =
        //        type.Transitions.Where(t => t.SourceState.IsEqv(state))
        //            .ToList();

        //    foreach (var transition in transitionsForState)
        //    {
        //        //if (!state.Transitions.Contains(transition))
        //        state.Transitions.TryAddValue(transition);
        //    }
        //}
    }
}

