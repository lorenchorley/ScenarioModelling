﻿namespace ScenarioModelling.Serialisation.Parsers;

public interface IParserResult<TNode>
{
    public TNode? ParsedObject { get; }
    public List<string> Errors { get; }
    public bool HasErrors { get; }
}