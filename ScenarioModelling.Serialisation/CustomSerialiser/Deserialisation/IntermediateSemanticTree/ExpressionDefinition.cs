namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;

public record ExpressionDefinition : ParentDefinition
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
