using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Serialisation.Parsers;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.Interpreter;

public class CustomSerialiserParserResult : IParserResult<Definitions>
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
