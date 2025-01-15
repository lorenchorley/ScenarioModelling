﻿namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public abstract record Definition
{
    public Definition? ParentDefinition { get; set; }
    public int? Line { get; set; }
}
