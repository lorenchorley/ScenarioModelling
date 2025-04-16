using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, AssertNode>]
public class AssertNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "Assert".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public List<IStoryNodeWithExpression> ConditionsToInitialise = new();

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, IStoryNode? existingCorrespondingNode, TryTransformDefinitionToNodeDelegate transformDefinition)
    {
        if (def is not ExpressionDefinition expDef)
        {
            throw new Exception("Assert node must be expression definition");
        }

        if (existingCorrespondingNode != null && existingCorrespondingNode is not AssertNode)
        {
            throw new Exception(@$"While trying to transform the definition ""{def}"" into a node of type {nameof(AssertNode)}, the type did not match with an existing node of type {existingCorrespondingNode.GetType().Name} in the subgraph");
        }


        def.HasBeenTransformed = true;

        AssertNode node = new();
        node.Line = def.Line;

        ExpressionInterpreter interpreter = new();

        var result = interpreter.Parse(expDef.Block.ExpressionText.Value);

        if (result.HasErrors)
        {
            throw new ExpressionException($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on while node{node.LineInformation} : \n{result.Errors.CommaSeparatedList()}");
        }

        if (result.ParsedObject is null)
        {
            throw new InternalLogicException($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on while node{node.LineInformation} : return value not set");
        }

        node.OriginalExpressionText = expDef.Block.ExpressionText.Value;
        node.AssertionExpression = result.ParsedObject;
        ConditionsToInitialise.Add(node);

        foreach (var item in expDef.Definitions)
        {
            if (item is NamedDefinition named && named.Type.IsEqv("MetaStoryName"))
            {
                item.HasBeenTransformed = true;
            }
        }

        return node;
    }
}

