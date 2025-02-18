namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record ExpressionDefinition : ParentalDefinition
{
    public StringValue Name { get; set; } = null!;
    public ExpressionBlock Block { get; set; } = null!;
    
    public override string ToEssentialString()
        => $"ExpressionDefinition({Name}, {Block})";

    public override string ToString()
    {
        if (Definitions.Count == 0)
        {
            return ToEssentialString();
        }
        else
        {
            return $"{ToEssentialString()} {{ {Definitions} }}";
        }
    }
}
