namespace ScenarioModel.Serialisation.HumanReadable.SemanticTree;

public record TransitionDefinition : UnnamedDefinition
{
    public StringValue TransitionName { get; init; } = null!;

    public override string ToString()
    {
        return $"Definition({Type} : {TransitionName})";
    }
}
