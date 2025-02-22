namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;

public record ExpressionBlock
{
    public StringValue ExpressionText { get; set; } = null!;

    public override string ToString()
    {
        return $"<{ExpressionText}>";
    }
}
