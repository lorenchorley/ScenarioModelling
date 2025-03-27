using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, IfNode>]
public class IfNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "If".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public List<IStoryNodeWithExpression> ConditionsToInitialise = new();

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IStoryNode>, Option<IStoryNode>> tryTransform)
    {
        if (def is not ExpressionDefinition expDef)
        {
            throw new Exception("If node must be expression definition");
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

        //node.SubGraph.ParentSubgraph = currentSubgraph;
        node.SubGraph.AddRangeToSequence(expDef.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, node.SubGraph), "Unknown node types not taken into account : {0}").ToList());

        return node;
    }
}

