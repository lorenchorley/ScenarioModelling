﻿using ScenarioModel.Parsers;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.Interpreter;

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
