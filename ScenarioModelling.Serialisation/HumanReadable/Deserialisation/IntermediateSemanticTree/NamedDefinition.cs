namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record NamedDefinition : UnnamedDefinition
{
    public StringValue Name { get; set; } = null!;

    public override string ToEssentialString()
        => $"NamedDefinition({Type}, {Name})";

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
