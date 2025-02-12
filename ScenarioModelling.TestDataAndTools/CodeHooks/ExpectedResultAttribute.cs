namespace ScenarioModelling.TestDataAndTools.CodeHooks;

public class ExpectedResultAttribute : Attribute
{
    public string Text { get; }

    public ExpectedResultAttribute(string text)
    {
        Text = text;
    }
}
