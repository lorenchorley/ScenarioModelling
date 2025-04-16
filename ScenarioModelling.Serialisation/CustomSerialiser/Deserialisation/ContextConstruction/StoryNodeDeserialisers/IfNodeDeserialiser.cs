using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, IfNode>]
public class IfNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "If".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public List<IStoryNodeWithExpression> ConditionsToInitialise = new();

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, IStoryNode? existingCorrespondingNode, TryTransformDefinitionToNodeDelegate tryTransform)
    {
        if (def is not ExpressionDefinition expDef)
        {
            throw new Exception("If node must be expression definition");
        }

        if (existingCorrespondingNode != null && existingCorrespondingNode is not IfNode)
        {
            throw new Exception(@$"While trying to transform the definition ""{def}"" into a node of type {nameof(IfNode)}, the type did not match with an existing node of type {existingCorrespondingNode.GetType().Name} in the subgraph");
        }

        def.HasBeenTransformed = true;

        IfNode node = new();
        node.Line = def.Line;

        ExpressionInterpreter interpreter = new();

        var result = interpreter.Parse(expDef.Block.ExpressionText.Value);

        if (result.HasErrors)
            throw new ExpressionException($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node{node.LineInformation} : \n{result.Errors.CommaSeparatedList()}");

        if (result.ParsedObject is null)
            throw new InternalLogicException($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node{node.LineInformation} : return value not set");

        node.OriginalConditionText = expDef.Block.ExpressionText.Value;
        node.AssertionExpression = result.ParsedObject;
        ConditionsToInitialise.Add(node);

        var subgraphToUse =
            existingCorrespondingNode != null
            ? ((IfNode)existingCorrespondingNode).SubGraph // If there's an existing node, we take it's subgraph so as to be able to compare the transformed nodes with the existing ones the whole way down
            : node.SubGraph; // If there is no existing node, we use the new one

        subgraphToUse.TransformAndMergeDefinitionsIntoSubgraph(expDef, tryTransform);

        return node;
    }
}

