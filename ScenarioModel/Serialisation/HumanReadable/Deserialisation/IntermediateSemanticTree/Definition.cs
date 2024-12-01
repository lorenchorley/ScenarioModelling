namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public abstract record Definition
{
    public Definition? ParentDefinition { get; init; }
    public int? Line { get; init; }
}
