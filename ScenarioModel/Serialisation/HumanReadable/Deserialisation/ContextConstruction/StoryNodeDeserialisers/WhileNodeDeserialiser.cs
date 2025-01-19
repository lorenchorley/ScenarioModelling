using LanguageExt;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Expressions.Interpreter;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, WhileNode>]
public class WhileNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "While".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public List<IStoryNodeWithExpression> ConditionsToInitialise = new();

    public IStoryNode Transform(Definition def, MetaStory MetaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IStoryNode>, Option<IStoryNode>> tryTransform)
    {
        if (def is not ExpressionDefinition expDef)
        {
            throw new Exception("If node must be expression definition");
        }

        WhileNode node = new();
        node.Line = def.Line;

        ExpressionInterpreter interpreter = new();

        var result = interpreter.Parse(expDef.Block.ExpressionText.Value);

        if (result.HasErrors)
        {
            throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on while node{node.LineInformation} : \n{result.Errors.CommaSeparatedList()}");
        }

        if (result.ParsedObject is null)
        {
            throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" on while node{node.LineInformation} : return value not set");
        }

        node.Condition = result.ParsedObject;
        ConditionsToInitialise.Add(node);

        node.SubGraph.ParentSubgraph = currentSubgraph;
        //node.SubGraph.ReentryPoint = node; // Node is probaby wrong here
        node.SubGraph.NodeSequence.AddRange(expDef.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, node.SubGraph), "Unknown node types not taken into account : {0}").ToList());

        return node;
    }
}

