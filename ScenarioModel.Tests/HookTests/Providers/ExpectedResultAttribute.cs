namespace ScenarioModelling.Tests.HookTests.Providers;

public class ExpectedResultAttribute : Attribute
{
    public string Text { get; }

    public ExpectedResultAttribute(string text)
    {
        Text = text;
    }
}
