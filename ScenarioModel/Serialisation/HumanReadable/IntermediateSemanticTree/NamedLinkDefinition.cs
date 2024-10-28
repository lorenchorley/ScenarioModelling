namespace ScenarioModel.Serialisation.HumanReadable.SemanticTree;

public record NamedLinkDefinition : UnnamedLinkDefinition
{
    public StringValue Name { get; init; } = null!;

    public override string ToString()
    {
        return $"Link({Source} -> {Destination}, {Name.Value})";
    }
}
