using System.Reflection;

namespace ScenarioModelling.TestDataAndTools.Serialisation;

public class ReserialisationDataProviderAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => Enumerable.Empty<IContextReserialisationTestCase>()
                     .Concat(ReserialisationTestData.GetTestCases())
                     .ThrowIfDuplicatesDetected(t => t.Name)
                     .Select(TransformIntoArgumentsArray);

    private static object[] TransformIntoArgumentsArray(IContextReserialisationTestCase testCase)
    {
        if (testCase is CompleteTestCase completeTestCase)
            return [completeTestCase.Name, completeTestCase.text, completeTestCase.text];

        if (testCase is IncompleteTestCase incompleteTestCase)
            return [incompleteTestCase.Name, incompleteTestCase.originalText, incompleteTestCase.expectedFinalText];

        throw new NotImplementedException();
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => data?[0]?.ToString() ?? "";

}