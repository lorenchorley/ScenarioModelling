namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record UnnamedDefinition : Definition
{
    public StringValue Type { get; set; } = null!;
    public Definitions Definitions { get; set; } = new();

    public override string ToString()
    {
        if (Definitions.Count == 0)
        {
            return $"Definition({Type}, _)";
        }
        else
        {
            return $"Definition({Type}, _) {{ {Definitions} }}";
        }
    }
}
