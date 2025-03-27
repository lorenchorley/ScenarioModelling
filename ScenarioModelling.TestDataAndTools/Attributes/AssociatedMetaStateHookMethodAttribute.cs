namespace ScenarioModelling.TestDataAndTools.Attributes;

public class AssociatedMetaStateHookMethodAttribute : Attribute
{
    public string MethodName { get; }

    public AssociatedMetaStateHookMethodAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
