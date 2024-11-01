using ScenarioModel.Serialisation.HumanReadable.SemanticTree;
using ScenarioModel.Parsers;

namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public class HumanReadableParserResult : IParserResult<Definitions>
{
    public Definitions? ParsedObject { get; set; }

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
