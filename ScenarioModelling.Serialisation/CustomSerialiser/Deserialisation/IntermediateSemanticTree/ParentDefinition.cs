namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;

public abstract record ParentDefinition : Definition
{
    public Definitions Definitions { get; set; } = new();
}
