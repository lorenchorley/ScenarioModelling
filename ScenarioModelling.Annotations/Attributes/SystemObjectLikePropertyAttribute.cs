namespace ScenarioModelling.Annotations.Attributes;

public class SystemObjectLikePropertyAttribute : Attribute, IPropertyAttribute
{
    public bool Serialise { get; } = true;
    public string? SerialisedName { get; }
    public bool DoNotSerialiseIfNullOrEmpty { get; }

    public SystemObjectLikePropertyAttribute()
    {

    }

    public SystemObjectLikePropertyAttribute(bool serialise = true)
    {
        Serialise = serialise;
    }

    public SystemObjectLikePropertyAttribute(string? serialisedName = null, bool doNotSerialiseIfNullOrEmpty = true)
    {
        SerialisedName = serialisedName;
        DoNotSerialiseIfNullOrEmpty = doNotSerialiseIfNullOrEmpty;
    }

}
