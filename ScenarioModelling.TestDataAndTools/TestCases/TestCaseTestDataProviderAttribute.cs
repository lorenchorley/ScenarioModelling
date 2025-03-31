using ScenarioModelling.TestDataAndTools.Attributes;
using System.Diagnostics;
using System.Reflection;

namespace ScenarioModelling.TestDataAndTools.TestCases;

public class TestCaseTestDataProviderAttribute : Attribute, ITestDataSource
{
    public static readonly string PrimaryMetaStoryName = "MetaStory recorded by hooks";
    public static readonly string PrimaryTestCaseName = "Main test case";

    private record TestCaseTestCase(string MetaStoryMethodName, bool TestDefinedFirstMetaStory = true);

    private List<TestCaseTestCase> TestData = [
            new(nameof(TestCaseTestData.NoStateChanges_NoAssertions_Succeeds)),
            new(nameof(TestCaseTestData.OneStateChange_OneAssertion_Succeeds)),
            new(nameof(TestCaseTestData.OneStateChange_InitialStateDifferent_NoAssertions_Succeeds)),
            new(nameof(TestCaseTestData.OneStateChange_InitialStateDifferent_OneAssertion_Succeeds)),
            new(nameof(TestCaseTestData.OneState_MultipleInitialStates_OneAssertion_Fails_OnUnknownEntity)),
            new(nameof(TestCaseTestData.OneState_MultipleInitialStates_OneAssertion_Fails_OnEntityWithoutState)),
            new(nameof(TestCaseTestData.OneState_MultipleAssertions_Fails_OnUnknownEntity)),
            new(nameof(TestCaseTestData.OneState_MultipleAssertions_Fails_OnEntityWithoutState)),
            new(nameof(TestCaseTestData.MultipleStates_OneAssertion_Succeeds)),
        ];

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select((Func<TestCaseTestCase, object[]>)(t => [t.MetaStoryMethodName, GetAssociatedMetaStateMethodName(t.MetaStoryMethodName), t.TestDefinedFirstMetaStory]));

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data) => data?[0]?.ToString() ?? "";

    [DebuggerNonUserCode]
    public static Action<T> GetAction<T>(string systemMethodName)
    {
        var methodRef =
            typeof(TestCaseTestData)
                .GetMethod(systemMethodName, BindingFlags.Public | BindingFlags.Static)
                ?? throw new Exception($"Method {systemMethodName} not found in {nameof(TestCaseTestData)}");

        return (parameter) => methodRef.Invoke(typeof(TestCaseTestData), [parameter]);
    }

    public static string GetExpectedContextText(string metaStateMethodName, string metaStoryMethodName, bool testDefinedFirstMetaStory)
    {
        var metaState = GetExpectedText(metaStateMethodName);

        string metaStoryText = GetExpectedText(metaStoryMethodName);

        metaStoryText = testDefinedFirstMetaStory ?
            metaStoryText
            : $$"""
            MetaStory "{{PrimaryMetaStoryName}}" {
            {{metaStoryText.AddIndent("  ")}}
            }
            """;

        return $"""
            {metaState}

            {metaStoryText}
            """.Trim();
    }

    public static string GetExpectedText(string metaStoryMethodName)
    {
        var attr = GetAssociatedExpectedText<ExpectedSerialisedFormAttribute>(metaStoryMethodName);
        return attr?.Text ?? $"Text not set. To set this text, apply the attribute ExpectedSerialisedForm to the hook method {metaStoryMethodName}.";
    }

    private static string GetAssociatedMetaStateMethodName(string metaStateMethodName)
    {
        var attr = GetAssociatedExpectedText<AssociatedMetaStateHookMethodAttribute>(metaStateMethodName);
        return attr?.MethodName ?? $"Method name not set. To set this name, apply the attribute AssociatedMetaStateHookMethod to the hook method {metaStateMethodName}.";
    }

    private static T? GetAssociatedExpectedText<T>(string metaStoryMethodName) where T : Attribute
    {
        var methodRef =
            typeof(TestCaseTestData)
                .GetMethod(metaStoryMethodName, BindingFlags.Public | BindingFlags.Static)
                ?? throw new Exception($"Method {metaStoryMethodName} not found in {nameof(TestCaseTestData)}");

        return methodRef.GetCustomAttribute<T>();
    }
}
