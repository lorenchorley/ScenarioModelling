namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record StringValue
{
    public string Value { get; set; } = null!;

    public override string ToString()
    {
        return Value;
    }
}
