using ScenarioModel.Serialisation.HumanReadable.SemanticTree;
using LanguageExt;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.Collections;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

public class StateTransitionNodeProfile : ISemanticNodeProfile
{
    public string Name => "Transition".ToUpperInvariant();

    public Func<Definition, bool>? Predicate => (def) =>
    {
        return def is TransitionDefinition;
    };

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> transformDefinition)
    {
        StateTransitionNode node = new();
        node.Line = def.Line;

        Definitions? defs = null;

        if (def is UnnamedDefinition unnamed)
        {
            defs = unnamed.Definitions;
        }
        else if (def is NamedLinkDefinition namedLink)
        {
            node.TransitionName = namedLink.Name.Value;
            //node. namedLink.Source
            throw new NotImplementedException();

        }
        else if (def is UnnamedLinkDefinition unnamedLink)
        {
            throw new NotImplementedException();
        }
        else if (def is TransitionDefinition transition)
        {
            node.TransitionName = transition.TransitionName.Value;
            throw new NotImplementedException();
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

