namespace ScenarioModelling.TestDataAndTools.Attributes;

public enum SerialisationType
{
    CustomSerialiser,
    Yaml
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ExpectedSerialisedFormAttribute : Attribute
{
    public string Text { get; }
    public SerialisationType SerialisationType { get; }

    public ExpectedSerialisedFormAttribute(string text, SerialisationType serialisationType = SerialisationType.CustomSerialiser)
    {
        Text = text;
        SerialisationType = serialisationType;
    }
}
