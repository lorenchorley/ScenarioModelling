namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record UnnamedDefinition : ParentalDefinition
{
    public StringValue Type { get; set; } = null!;

    public override string ToEssentialString()
    {
        return $"UnnamedDefinition({Type})";
    }

    public override string ToString()
    {
        if (Definitions.Count == 0)
        {
            return ToEssentialString();
        }
        else
        {
            return $"{ToEssentialString()} {{ {Definitions} }}";
        }
    }
}
