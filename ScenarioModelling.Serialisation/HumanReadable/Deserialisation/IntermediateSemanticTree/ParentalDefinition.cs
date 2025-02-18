namespace ScenarioModelling.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;

public abstract record ParentalDefinition : Definition
{
    public Definitions Definitions { get; set; } = new();
}
