namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record NamedLinkDefinition : UnnamedLinkDefinition
{
    public StringValue Name { get; init; } = null!;

    public override string ToString()
    {
        return $"Link({Source} -> {Destination}, {Name.Value})";
    }
}
