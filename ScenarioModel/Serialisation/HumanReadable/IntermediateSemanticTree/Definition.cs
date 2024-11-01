namespace ScenarioModel.Serialisation.HumanReadable.SemanticTree;

public abstract record Definition
{
    public Definition? ParentDefinition { get; init; }
    public int? Line { get; init; }
}
