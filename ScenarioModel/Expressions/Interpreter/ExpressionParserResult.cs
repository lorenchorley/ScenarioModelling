using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Parsers;

namespace ScenarioModel.Expressions.Interpreter;

public class ExpressionParserResult : IParserResult<Expression>
{
    public Expression ParsedObject { get; set; }

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
        return ParsedObject.ToString();
    }
}
