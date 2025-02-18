namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record TransitionDefinition : UnnamedDefinition
{
    public StringValue TransitionName { get; set; } = null!;

    public override string ToEssentialString()
        => ToString();

    public override string ToString()
    {
        return $"TransitionDefinition({Type} -> {TransitionName})";
    }
}
