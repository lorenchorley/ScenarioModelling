using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers;

[MetaStateObjectLike<IDefinitionToObjectDeserialiser, Transition>]
public class TransitionDeserialiser(MetaState MetaState, Instanciator Instanciator) : DefinitionToObjectDeserialiser<Transition, TransitionReference>
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

        def.HasBeenTransformed = true;

        if (type != TransformationType.Property)
            throw new Exception("A transition must always be the propert of another object");

        Transition value = Instanciator.NewUnregistered<Transition>(definition: def);

        value.SourceState.SetReference(Instanciator.NewReference<State, StateReference>(name: unnamed.Source.Value));
        value.DestinationState.SetReference(Instanciator.NewReference<State, StateReference>(name: unnamed.Destination.Value));

        if (MetaState.Transitions.Any(e => e.IsEqv(value) && value.SourceState.Name.IsEqv(e.SourceState.Name) && value.DestinationState.Name.IsEqv(e.DestinationState.Name)))
        {
            // If an object of the same type with the same name already exists,
            // we remove this one and but return the object as if it we've transformed so that it doesn't get signaled as not transformed
            return value.GenerateReference();
        }

        Instanciator.RegisterWithMetaState(value);
        return value.GenerateReference();
    }

    public override void BeforeIndividualInitialisation()
    {
        // TODO For each state on each transition, we need to associate them with their state machine
        foreach (var transition in MetaState.Transitions)
        {
            StateMachine? sm =
                MetaState.StateMachines
                      .Where(sm => NewMethod(sm, transition))
                      .Single();

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

    private static bool NewMethod(StateMachine sm, Transition transition)
    {
        return sm.Transitions.Any(e => NewMethod1(transition, e));
    }

    private static bool NewMethod1(Transition transition, Transition e)
    {
        if (!e.IsEqv(transition))
            return false;

        if (!transition.SourceState.Name.IsEqv(e.SourceState.Name))
            return false;

        if (!transition.DestinationState.Name.IsEqv(e.DestinationState.Name))
            return false;

        return true;
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

