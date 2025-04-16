using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, TransitionNode>]
public class TransitionNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "Transition".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => (def) =>
    {
        return def is TransitionDefinition;
    };

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, IStoryNode? existingCorrespondingNode, TryTransformDefinitionToNodeDelegate transformDefinition)
    {
        TransitionNode node = new();
        node.Line = def.Line;

        def.HasBeenTransformed = true;

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

        if (existingCorrespondingNode != null && existingCorrespondingNode is not TransitionNode)
        {
            throw new Exception(@$"While trying to transform the definition ""{def}"" into a node of type {nameof(TransitionNode)}, the type did not match with an existing node of type {existingCorrespondingNode.GetType().Name} in the subgraph");
        }


        if (defs != null)
        {
            foreach (var item in defs)
            {
                if (item is TransitionDefinition transitionDefinition)
                {
                    node.TransitionName = transitionDefinition.TransitionName.Value;

                    IStateful? stateful = metaStory.MetaState.AllStateful.FirstOrDefault(e => StatefulObjectHasName(e, transitionDefinition.Type.Value));

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

    private static bool StatefulObjectHasName(IStateful stateful, string name)
    {
        if (stateful is Aspect aspect)
        {
            string aspectName = $"{aspect.Entity.Name}.{aspect.Name}";
            return aspectName.IsEqv(name);
        }
        else
        {
            return stateful.Name.IsEqv(name);
        }
    }
}

