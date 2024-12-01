namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record StringValue
{
    public string Value { get; init; } = null!;

    public override string ToString()
    {
        return Value;
    }
}
