using LanguageExt;
using ScenarioModel.Collections.Graph;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers.Intefaces;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers;

[NodeLike<IDefinitionToNodeDeserialiser, IfNode>]
public class IfNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [NodeLikeProperty]
    public string Name => "If".ToUpperInvariant();

    [NodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public List<IScenarioNodeWithExpression> ConditionsToInitialise = new();

    public IScenarioNode Transform(Definition def, MetaStory scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> tryTransform)
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

