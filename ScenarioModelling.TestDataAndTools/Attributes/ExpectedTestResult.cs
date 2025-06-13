
namespace ScenarioModelling.TestDataAndTools.Attributes;

public class ExpectedTestFailureResult : Attribute
{
    public string Reason { get; }
    public bool IsError { get; }

    public ExpectedTestFailureResult(string reason, bool isError = false)
    {
        Reason = reason;
        IsError = isError;
    }
}
