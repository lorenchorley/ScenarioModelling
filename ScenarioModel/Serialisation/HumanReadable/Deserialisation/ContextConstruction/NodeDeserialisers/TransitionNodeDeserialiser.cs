﻿using LanguageExt;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers;

[NodeLike<IDefinitionToNodeDeserialiser, TransitionNode>]
public class TransitionNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [NodeLikeProperty]
    public string Name => "Transition".ToUpperInvariant();

    [NodeLikeProperty]
    public Func<Definition, bool>? Predicate => (def) =>
    {
        return def is TransitionDefinition;
    };

    public IScenarioNode Transform(Definition def, MetaStory scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> transformDefinition)
    {
        TransitionNode node = new();
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

                    node.StatefulObject = stateful.GenerateStatefulReference();
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

