namespace ScenarioModelling.TestDataAndTools.Attributes;

public class ExpectedResultAttribute : Attribute
{
    public string Text { get; }

    public ExpectedResultAttribute(string text)
    {
        Text = text;
    }
}
