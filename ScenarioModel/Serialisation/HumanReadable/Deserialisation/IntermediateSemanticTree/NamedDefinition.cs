namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record NamedDefinition : UnnamedDefinition
{
    public StringValue Name { get; init; } = null!;

    public override string ToString()
    {
        if (Definitions.Count == 0)
        {
            return $"Definition({Type}, {Name})";
        }
        else
        {
            return $"Definition({Type}, {Name}) {{ {Definitions} }}";
        }
    }
}
