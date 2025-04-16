using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;
using LanguageExt;

public static class SubGraphExtensions
{
    public static void TransformAndMergeDefinitionsIntoSubgraph(this SemiLinearSubGraph<IStoryNode> subgraph, ParentDefinition def, TryTransformDefinitionToNodeDelegate tryTransform)
    {
        if (subgraph.NodeSequence.Count == 0)
        {
            // If the existing list is empty, populate it with the incoming list
            subgraph.AddRangeToSequence(def.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, subgraph, null), "Unknown node types not taken into account : {0}"));
        }
        else if (def.Definitions.Count == 0)
        {
            // There are no incoming definitions, but there are existing nodes.
            // The existing nodes should stay since they have already been established and we have nothing to add to check from the incoming definitions
            // So there's nothing to do
        }
        else // Multiple definitions and multiple existing nodes
        {
            // TODO Take into implicit nodes that might not appear in one sequence or the other, but must be merged from both. Changes to everything below will be necessary

            // Ensure the incoming list corresponds to the existing sequence in length
            // If they don't correspond, we can't merge them unless there are implicit nodes
            if (def.Definitions.Count != subgraph.NodeSequence.Count)
            {
                throw new ContextMergingException($@"The definition ""{def}"" does not have a number of child nodes ({def.Definitions.Count}) the same as the subgraph at the same level ({subgraph.NodeSequence.Count})");
            }

            foreach (var (existingCorrespondingNode, incomingDefinition) in subgraph.NodeSequence.CombinePairwise(def.Definitions))
            {
                Option<IStoryNode> result = tryTransform(incomingDefinition, subgraph, existingCorrespondingNode);

                result.Match(
                    Some: node => {
                        if (!node.IsFullyEqv(existingCorrespondingNode))
                        {
                            throw new ContextMergingException($@"The definition ""{incomingDefinition}"" was transformed into the node ""{node}"" which is not fully equivalent to the existing node ""{existingCorrespondingNode}""");
                        }
                    },
                    None: () => throw new ContextMergingException($@"The definition ""{incomingDefinition}"" could not be transformed into a node of type ""{existingCorrespondingNode.GetType().Name}""")
                );

            }

        }
    }
}
