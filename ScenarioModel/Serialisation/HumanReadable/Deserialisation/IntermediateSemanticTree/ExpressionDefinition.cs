namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record ExpressionDefinition : Definition
{
    public StringValue Name { get; set; } = null!;
    public ExpressionBlock Block { get; set; } = null!;
    public Definitions Definitions { get; set; } = new();

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
