namespace ScenarioModel.Serialisation.HumanReadable.SemanticTree;

public record StringValue
{
    public string Value { get; init; } = null!;

    public override string ToString()
    {
        return Value;
    }
}
