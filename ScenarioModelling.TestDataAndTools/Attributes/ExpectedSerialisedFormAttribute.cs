namespace ScenarioModelling.TestDataAndTools.Attributes;

public class ExpectedSerialisedFormAttribute : Attribute
{
    public string Text { get; }

    public ExpectedSerialisedFormAttribute(string text)
    {
        Text = text;
    }
}
