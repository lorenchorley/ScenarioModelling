using ScenarioModelling.CoreObjects.Expressions.SemanticTree;
using ScenarioModelling.Serialisation.Parsers;

namespace ScenarioModelling.Serialisation.Expressions.Interpreter;

public class ExpressionParserResult : IParserResult<Expression>
{
    public Expression? ParsedObject { get; set; }

    public List<string> Errors { get; internal set; } = new();

    public bool HasErrors
    {
        get
        {
            return Errors.Count > 0;
        }
    }

    public override string ToString()
    {
        return ParsedObject?.ToString() ?? "<No result>";
    }
}
