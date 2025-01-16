using LanguageExt;
using ScenarioModelling.ContextConstruction;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.SystemObjectDeserialisers.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.SystemObjectDeserialisers;

[SystemObjectLike<IDefinitionToObjectDeserialiser, State>]
public class StateDeserialiser(System System, Instanciator Instanciator) : DefinitionToObjectDeserialiser<State, StateReference>
{
    /// <summary>
    /// Should return a reference. The instance should be recorded to the system by the class itself
    /// </summary>
    /// <param name="def"></param>
    /// <returns></returns>
    protected override Option<StateReference> Transform(Definition def, TransformationType type)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.IsEqv("State"))
        {
            return null;
        }

        // If this is meant to be the value of a property in another object, we need to return a reference
        // Otherwise we make a full object that is stored in the system
        if (type == TransformationType.Property)
            return Instanciator.NewReference<State, StateReference>(definition: def);

        State value = Instanciator.New<State>(definition: def);

        return value.GenerateReference();
    }

    public override void BeforeIndividualInitialisation()
    {

    }

    public override void Initialise(State obj)
    {
        // If state is an orphan state, attribute it a new state machine
        if (!System.StateMachines.Any(SM => SM.States.Any(s => s.IsEqv(obj))))
        {
            Instanciator.New<StateMachine>().States.TryAddValue(obj);
        }
    }
}

