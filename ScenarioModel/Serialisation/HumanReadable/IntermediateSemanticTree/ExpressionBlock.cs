namespace ScenarioModel.Serialisation.HumanReadable.SemanticTree;

public record ExpressionBlock
{
    public StringValue ExpressionText { get; init; } = null!;

    public override string ToString()
    {
        return $"<{ExpressionText}>";
    }
}
