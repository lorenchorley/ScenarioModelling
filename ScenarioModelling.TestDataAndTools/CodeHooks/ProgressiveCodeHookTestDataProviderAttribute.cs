using ScenarioModelling.TestDataAndTools.Attributes;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ScenarioModelling.TestDataAndTools.CodeHooks;

public class ProgressiveCodeHookTestDataProviderAttribute : Attribute, ITestDataSource
{
    public static readonly string PrimaryMetaStoryName = "MetaStory recorded by hooks";

    private record TestCase(string MetaStoryMethodName, bool TestDefinedFirstMetaStory = false, int LoopCount = 0);

    private List<TestCase> TestData = [
            new(nameof(CodeHookTestData.Assert_OneState_Succeeds)),
            new(nameof(CodeHookTestData.Assert_Transition_Fails)),
            new(nameof(CodeHookTestData.Assert_WithName_Transition_Fails)),
            new(nameof(CodeHookTestData.OneDialog)),
            new(nameof(CodeHookTestData.OneDialogWithMultipleWords)),
            new(nameof(CodeHookTestData.OneDialogWithId)),
            new(nameof(CodeHookTestData.OneDialogWithCharacter)),
            new(nameof(CodeHookTestData.TwoStatesOneTransition)),
            new(nameof(CodeHookTestData.TwoStatesOneTransitionWithId)),
            new(nameof(CodeHookTestData.OneConstraintAlwaysValid)),
            new(nameof(CodeHookTestData.OneConstraintFailsOnTransition)),
            new(nameof(CodeHookTestData.OneDialogAndOneJump)),
            new(nameof(CodeHookTestData.TwoDialogsAndOneJump)),
            new(nameof(CodeHookTestData.IfDoesNotExecute_DialogAfterOnly)),
            new(nameof(CodeHookTestData.IfDoesNotExecute_DialogBeforeAndAfter)),
            new(nameof(CodeHookTestData.IfDoesNotExecute_DialogBeforeOnly)),
            new(nameof(CodeHookTestData.IfDoesNotExecute_NoDialogAround)),
            new(nameof(CodeHookTestData.IfExecutesWithDialog_DialogAfterOnly)),
            new(nameof(CodeHookTestData.IfExecutesWithDialog_DialogBeforeOnly)),
            new(nameof(CodeHookTestData.IfExecutesWithDialog_DialogBeforeAndAfter)),
            new(nameof(CodeHookTestData.IfExecutesWithDialog_NoDialogAround)),
            new(nameof(CodeHookTestData.IfDoesNotExecute_HookOutsideBlock)),
            new(nameof(CodeHookTestData.IfExecutesWithDialog_HookOutsideBlock)),
            new(nameof(CodeHookTestData.IfDoesNotExecute_UsingHook)),
            new(nameof(CodeHookTestData.IfExecutesWithDialog_UsingHook)),
            new(nameof(CodeHookTestData.IfExecutes_ExpressionUsingAspectState)),
            new(nameof(CodeHookTestData.TwoNestedIfsThatExecute)),
            new(nameof(CodeHookTestData.TwoConsecutiveIfsThatExecute)),
            new(nameof(CodeHookTestData.LoopDoesNotExecute), LoopCount: 0),
            new(nameof(CodeHookTestData.LoopExecutesOnce), LoopCount: 1),
            new(nameof(CodeHookTestData.WhileDoesNotExecute)),
            new(nameof(CodeHookTestData.WhileExecutesOnceWithTransition)),
            new(nameof(CodeHookTestData.WhileExecutesTwiceWithTransition)),
            new(nameof(CodeHookTestData.WhileExecutesTwiceWithTransitionAndDialog)),
            new(nameof(CodeHookTestData.WhileExecutesTwiceWithNestedIf)),
            new(nameof(CodeHookTestData.WhileExecutesTwiceWithNestedIf_NoDialogAfter)),
            new(nameof(CodeHookTestData.CallMetaStory_OneLevel), TestDefinedFirstMetaStory: true),
            new(nameof(CodeHookTestData.CallMetaStory_OneLevel_TwoDifferentCalls), TestDefinedFirstMetaStory: true),
            new(nameof(CodeHookTestData.CallMetaStory_OneLevel_TwoCallsSameStory), TestDefinedFirstMetaStory: true),
            new(nameof(CodeHookTestData.CallMetaStory_TwoLevels), TestDefinedFirstMetaStory: true),
            new(nameof(CodeHookTestData.CallMetaStory_TwoLevelsCallSameTertiaryStory), TestDefinedFirstMetaStory: true),
            new(nameof(CodeHookTestData.CallMetaStory_ReiterateMainMetaStoryOnce), TestDefinedFirstMetaStory: true),
            new(nameof(CodeHookTestData.CallMetaStory_ReiterateOnceFromSecondaryStory), TestDefinedFirstMetaStory: true),
        ];

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select((Func<TestCase, object[]>)(t => [t.MetaStoryMethodName, GetAssociatedMetaStateMethodName(t.MetaStoryMethodName), t.TestDefinedFirstMetaStory, t.LoopCount]));

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data) => data?[0]?.ToString() ?? "";

    [DebuggerNonUserCode]
    public static Action<T> GetAction<T>(string systemMethodName)
    {
        var methodRef =
            typeof(CodeHookTestData)
                .GetMethod(systemMethodName, BindingFlags.Public | BindingFlags.Static)
                ?? throw new Exception($"Method {systemMethodName} not found in {nameof(CodeHookTestData)}");

        return (parameter) => methodRef.Invoke(typeof(CodeHookTestData), [parameter]);
    }

    public static string GetExpectedText(string metaStoryMethodName, SerialisationType serialisationType = SerialisationType.CustomSerialiser)
    {
        var attr = GetAssociatedExpectedText<ExpectedSerialisedFormAttribute>(metaStoryMethodName, attr => attr.SerialisationType == serialisationType);
        return attr?.Text ?? $"Text not set. To set this text, apply the attribute ExpectedSerialisedForm to the hook method {metaStoryMethodName}.";
    }

    public static string GetExpectedCustomContextText(string metaStateMethodName, string metaStoryMethodName, bool testDefinedFirstMetaStory)
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

    private static string GetAssociatedMetaStateMethodName(string metaStateMethodName)
    {
        var attr = GetAssociatedExpectedText<AssociatedMetaStateHookMethodAttribute>(metaStateMethodName);
        return attr?.MethodName ?? $"Method name not set. To set this name, apply the attribute AssociatedMetaStateHookMethod to the hook method {metaStateMethodName}.";
    }

    private static T? GetAssociatedExpectedText<T>(string metaStoryMethodName, Func<T, bool>? pred = null) where T : Attribute
    {
        var methodRef =
            typeof(CodeHookTestData)
                .GetMethod(metaStoryMethodName, BindingFlags.Public | BindingFlags.Static)
                ?? throw new Exception($"Method {metaStoryMethodName} not found in {nameof(CodeHookTestData)}");

        if (pred == null)
            return methodRef.GetCustomAttributes<T>().SingleOrDefault();
        else
            return methodRef.GetCustomAttributes<T>().Where(pred).SingleOrDefault();
    }

}
