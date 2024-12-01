namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record UnnamedLinkDefinition : Definition
{
    public StringValue Source { get; init; } = null!;
    public StringValue Destination { get; init; } = null!;

    public override string ToString()
    {
        return $"Link({Source} -> {Destination})";
    }
}
