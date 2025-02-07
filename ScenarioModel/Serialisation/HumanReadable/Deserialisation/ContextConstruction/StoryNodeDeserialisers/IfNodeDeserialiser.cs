using LanguageExt;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Interpreter;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

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

        IfNode node = new();
        node.Line = def.Line;

        ExpressionInterpreter interpreter = new();

        var result = interpreter.Parse(expDef.Block.ExpressionText.Value);

        if (result.HasErrors)
        {
            throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node{node.LineInformation} : \n{result.Errors.CommaSeparatedList()}");
        }

        if (result.ParsedObject is null)
        {
            throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on if node{node.LineInformation} : return value not set");
        }

        node.OriginalConditionText = expDef.Block.ExpressionText.Value;
        node.Condition = result.ParsedObject;
        ConditionsToInitialise.Add(node);

        node.SubGraph.ParentSubgraph = currentSubgraph;
        //node.SubGraph.ExplicitReentryPoint = node; // Node is probaby wrong here
        node.SubGraph.NodeSequence.AddRange(expDef.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, node.SubGraph), "Unknown node types not taken into account : {0}").ToList());

        return node;
    }
}

