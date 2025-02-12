namespace ScenarioModelling.TestDataAndTools.CodeHooks;

public class AssociatedSystemHookMethodAttribute : Attribute
{
    public string MethodName { get; }

    public AssociatedSystemHookMethodAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
