namespace ScenarioModelling.Exhaustiveness.Attributes;

public class ObjectLikePropertyAttribute : Attribute, IPropertyAttribute
{
    public bool Serialise { get; } = true;
    public string? SerialisedName { get; }
    public bool DoNotSerialiseIfNullOrEmpty { get; }

    public ObjectLikePropertyAttribute()
    {

    }

    public ObjectLikePropertyAttribute(bool serialise = true)
    {
        Serialise = serialise;
    }

    public ObjectLikePropertyAttribute(string? serialisedName = null, bool doNotSerialiseIfNullOrEmpty = true)
    {
        SerialisedName = serialisedName;
        DoNotSerialiseIfNullOrEmpty = doNotSerialiseIfNullOrEmpty;
    }

}
