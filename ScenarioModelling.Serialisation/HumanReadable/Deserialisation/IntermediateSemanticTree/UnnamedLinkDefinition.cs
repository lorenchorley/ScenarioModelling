namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record UnnamedLinkDefinition : Definition
{
    public StringValue Source { get; set; } = null!;
    public StringValue Destination { get; set; } = null!;
    public override string ToEssentialString()
        => $"UnnamedLinkDefinition({Source} -> {Destination})";

    public override string ToString()
    {
        return ToEssentialString();
    }
}
