namespace ScenarioModelling.TestDataAndTools.CodeHooks;

public class AssociatedMetaStateHookMethodAttribute : Attribute
{
    public string MethodName { get; }

    public AssociatedMetaStateHookMethodAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
