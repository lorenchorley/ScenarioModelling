using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using ScenarioModel.ScenarioObjects;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Serialisation.HumanReadable;

public class StateTransitionStepProfile : ISemanticStepProfile
{
    public string Name => "Transition";

    public Func<Definition, bool>? Predicate => (Definition def) =>
    {
        return def is TransitionDefinition;
    };

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario)
    {
        StateTransitionNode node = new();

        Definitions? defs = null;

        if (def is UnnamedDefinition unnamed)
        {
            defs = unnamed.Definitions;
        }
        else if (def is NamedLinkDefinition namedLink)
        {
            node.TransitionName = namedLink.Name.Value;
            //node. namedLink.Source

        }
        else if (def is UnnamedLinkDefinition unnamedLink)
        {

        }
        else if (def is TransitionDefinition transition)
        {
            node.TransitionName = transition.TransitionName.Value;

        }

        if (defs != null)
        {
            foreach (var item in defs)
            {
                if (item is TransitionDefinition transitionDefinition)
                {
                    node.TransitionName = transitionDefinition.TransitionName.Value;

                    IStateful? stateful = scenario.System.AllStateful.FirstOrDefault(e => e.Name.IsEqv(transitionDefinition.Type));

                    if (stateful == null)
                    {
                        throw new Exception($"No object {transitionDefinition.Type} found for transition");
                    }

                    node.StatefulObject = stateful.GenerateReference();
                    break;
                }
            }
        }

        if (node.StatefulObject == null)
        {
            throw new Exception($"No object was found for transition {node.TransitionName}");
        }

        return node;
    }
}

