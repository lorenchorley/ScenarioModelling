namespace ScenarioModelling.Exhaustiveness.Attributes;

public interface IPropertyAttribute
{
    bool Serialise { get; }
    string? SerialisedName { get; }
    bool DoNotSerialiseIfNullOrEmpty { get; }
}
