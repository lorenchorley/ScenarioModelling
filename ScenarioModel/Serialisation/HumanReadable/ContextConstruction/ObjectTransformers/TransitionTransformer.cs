using LanguageExt;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

[ObjectLike<IDefinitionToObjectTransformer, Transition>]
public class TransitionTransformer(System System, Instanciator Instanciator) : IDefinitionToObjectTransformer<Transition, TransitionReference>
{
    /// <summary>
    /// Should return a reference. The instance should be recorded to the system by the class itself
    /// </summary>
    /// <param name="def"></param>
    /// <returns></returns>
    public Option<TransitionReference> Transform(Definition def)
    {
        if (def is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.IsEqv("Transition"))
        {
            return null;
        }

        Transition value = Instanciator.New<Transition>(definition: def);

        return value.GenerateReference();

        // From state machine transformer
        //if (subdef is UnnamedLinkDefinition transition)
        //{
        //    Transition item = new(System)
        //    {
        //        SourceState = transition.Source.Value,
        //        DestinationState = transition.Destination.Value,
        //    };

        //    //Console.WriteLine($"Created Transition {item.Name}");

        //    if (subdef is NamedLinkDefinition nammedTransition)
        //    {
        //        item.Name = nammedTransition.Name.Value;
        //    }

        //    value.Transitions.TryAddValue(item);
        //}
    }

    public void Validate(Transition obj)
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

