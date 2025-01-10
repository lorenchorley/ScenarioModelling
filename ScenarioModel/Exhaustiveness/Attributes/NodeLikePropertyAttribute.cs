namespace ScenarioModel.Exhaustiveness.Attributes;

public enum OptionalBoolSetting
{
    NotOptional,
    TrueAsDefault,
    FalseAsDefault
}

public class NodeLikePropertyAttribute : Attribute, IPropertyAttribute
{
    public bool Serialise { get; } = true;
    public OptionalBoolSetting OptionalBool { get; } = OptionalBoolSetting.NotOptional;
    public string? SerialisedName { get; }
    public bool DoNotSerialiseIfNullOrEmpty { get; }

    /// <summary>
    /// All defaults
    /// </summary>
    public NodeLikePropertyAttribute()
    {
    }

    /// <summary>
    /// Specifically to set as not serialised
    /// </summary>
    /// <param name="serialise"></param>
    public NodeLikePropertyAttribute(bool serialise = true)
    {
        Serialise = serialise;
    }

    /// <summary>
    /// Allows for serialisation options
    /// </summary>
    /// <param name="serialisedName"></param>
    /// <param name="doNotSerialiseIfNullOrEmpty"></param>
    /// <param name="optionalBool"></param>
    public NodeLikePropertyAttribute(string? serialisedName = null, bool doNotSerialiseIfNullOrEmpty = true, OptionalBoolSetting optionalBool = OptionalBoolSetting.NotOptional)
    {
        SerialisedName = serialisedName;
        DoNotSerialiseIfNullOrEmpty = doNotSerialiseIfNullOrEmpty;
        OptionalBool = optionalBool;
    }

}
