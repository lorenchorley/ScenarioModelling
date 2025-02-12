namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public record TransitionDefinition : UnnamedDefinition
{
    public StringValue TransitionName { get; set; } = null!;

    public override string ToString()
    {
        return $"Definition({Type} : {TransitionName})";
    }
}
