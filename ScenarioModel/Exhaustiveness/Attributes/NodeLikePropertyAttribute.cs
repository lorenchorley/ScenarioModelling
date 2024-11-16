namespace ScenarioModel.Exhaustiveness.Attributes;

public class NodeLikePropertyAttribute : Attribute, IPropertyAttribute
{
    public bool Serialise { get; } = true;
    public string? SerialisedName { get; }
    public bool DoNotSerialiseIfNullOrEmpty { get; }

    public NodeLikePropertyAttribute()
    {

    }

    public NodeLikePropertyAttribute(bool serialise = true)
    {
        Serialise = serialise;
    }

    public NodeLikePropertyAttribute(string? serialisedName = null, bool doNotSerialiseIfNullOrEmpty = true)
    {
        SerialisedName = serialisedName;
        DoNotSerialiseIfNullOrEmpty = doNotSerialiseIfNullOrEmpty;
    }

}
