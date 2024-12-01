namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record ExpressionBlock
{
    public StringValue ExpressionText { get; init; } = null!;

    public override string ToString()
    {
        return $"<{ExpressionText}>";
    }
}
