namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;

public record NamedLinkDefinition : UnnamedLinkDefinition
{
    public StringValue Name { get; set; } = null!;

    public override string ToEssentialString()
        => ToString();

    public override string ToString()
    {
        return $"NamedLinkDefinition({Source} -> {Destination}, {Name.Value})";
    }
}
