//using ScenarioModelling.CodeHooks;
//using ScenarioModelling.CodeHooks.HookDefinitions;
//using System.Diagnostics;
//using System.Reflection;

//namespace ScenarioModelling.Tests.HookTests;

//public class ProgressiveSerialisationTestDataProviderAttribute : Attribute, ITestDataSource
//{
//    private record TestCase(string MetaStoryMethodName, string SystemMethodName);
        
//    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
//    {
//        ProgressiveCodeHookTestDataProviderAttribute
//        TestData.Select((Func<TestCase, object[]>)(t => [t.MetaStoryMethodName, t.SystemMethodName]));

//    }

//    public string GetDisplayName(MethodInfo methodInfo, object?[]? data) => data?[0]?.ToString() ?? "";

//    [DebuggerNonUserCode]
//    public static Action<T> GetAction<T>(string systemMethodName)
//    {
//        var methodRef =
//            typeof(ProgressiveCodeHookTestDataProviderAttribute)
//                .GetMethod(systemMethodName, BindingFlags.NonPublic | BindingFlags.Static)
//                ?? throw new Exception($"Method {systemMethodName} not found in {nameof(ProgressiveCodeHookTestDataProviderAttribute)}");

//        return (T parameter) => methodRef.Invoke(typeof(ProgressiveCodeHookTestDataProviderAttribute), [parameter]);
//    }

//}
