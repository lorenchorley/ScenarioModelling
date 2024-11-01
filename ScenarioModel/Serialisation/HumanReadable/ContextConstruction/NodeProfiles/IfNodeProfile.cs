using ScenarioModel.Serialisation.HumanReadable.SemanticTree;
using LanguageExt;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Expressions.Validation;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Collections;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;

public class IfNodeProfile : ISemanticNodeProfile
{
    public string Name => "If".ToUpperInvariant();

    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, SemiLinearSubGraph<IScenarioNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> tryTransform)
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
            throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" : \n{result.Errors.CommaSeparatedList()}");
        }

        node.Condition = result.ParsedObject;

        ExpressionValidator visitor = new(scenario.System);
        node.Condition.Accept(visitor);

        if (visitor.Errors.Any())
        {
            throw new Exception($"Expression on if node was not valid : \n" + visitor.Errors.CommaSeparatedList());
        }

        node.SubGraph.ParentSubGraph = currentSubgraph;
        node.SubGraph.ParentSubGraphEntryPoint = node;
        node.SubGraph.NodeSequence.AddRange(expDef.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, node.SubGraph), "Unknown node types not taken into account : {0}").ToList());

        return node;
    }
}

