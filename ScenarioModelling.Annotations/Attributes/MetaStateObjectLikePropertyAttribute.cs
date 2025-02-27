namespace ScenarioModelling.Annotations.Attributes;

public class MetaStateObjectLikePropertyAttribute : Attribute, IPropertyAttribute
{
    public bool Serialise { get; } = true;
    public string? SerialisedName { get; }
    public bool DoNotSerialiseIfNullOrEmpty { get; }

    public MetaStateObjectLikePropertyAttribute()
    {

    }

    public MetaStateObjectLikePropertyAttribute(bool serialise = true)
    {
        Serialise = serialise;
    }

    public MetaStateObjectLikePropertyAttribute(string? serialisedName = null, bool doNotSerialiseIfNullOrEmpty = true)
    {
        SerialisedName = serialisedName;
        DoNotSerialiseIfNullOrEmpty = doNotSerialiseIfNullOrEmpty;
    }

}
