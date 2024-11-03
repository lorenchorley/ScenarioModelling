namespace ScenarioModel.Serialisation.HumanReadable.SemanticTree;

public record ExpressionDefinition : Definition
{
    public StringValue Name { get; init; } = null!;
    public ExpressionBlock Block { get; init; } = null!;
    public Definitions Definitions { get; init; } = new();

    public override string ToString()
    {
        if (Definitions.Count > 0)
        {
            return $"ExpressionDefinition({Name}, <{Block.ExpressionText}>) {{ {Definitions} }}";
        }
        else
        {
            return $"ExpressionDefinition({Name}, <{Block.ExpressionText}>)";
        }
    }
}
