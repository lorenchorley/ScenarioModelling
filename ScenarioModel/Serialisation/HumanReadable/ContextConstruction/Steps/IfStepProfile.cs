using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using LanguageExt;
using ScenarioModel.Expressions.Interpreter;
using ScenarioModel.Expressions.Validation;
using ScenarioModel.ScenarioObjects;
using System.Net.WebSockets;

namespace ScenarioModel.Serialisation.HumanReadable.ContextConstruction.Steps;

public class IfStepProfile : ISemanticStepProfile
{
    public string Name => "If".ToUpperInvariant();

    public Func<Definition, bool>? Predicate => null;

    public IScenarioNode CreateAndConfigure(Definition def, Scenario scenario, Func<Definition, Option<IScenarioNode>> transformDefinition)
    {
        if (def is not ExpressionDefinition expDef)
        {
            throw new Exception("If node must be expression definition");
        }

        IfNode node = new();

        ExpressionInterpreter interpreter = new();

        var result = interpreter.Parse(expDef.Block.ExpressionText.Value);

        

        if (result.HasErrors)
        {
            throw new Exception($@"Unable to parse expression ""{expDef.Block.ExpressionText.Value}"" : \n{result.Errors.CommaSeparatedList()}");
        }

        node.Expression = result.ParsedObject;

        ValidatorVisitor visitor = new(scenario.System);
        node.Expression.Accept(visitor);

        if (visitor.Errors.Any())
        {
            throw new Exception($"Expression on if node was not valid : \n" + visitor.Errors.CommaSeparatedList());
        }

        node.Steps = expDef.Definitions.ChooseAndAssertAllSelected(transformDefinition, "Unknown step types not taken into account : {0}").ToList();

        return node;
    }
}

