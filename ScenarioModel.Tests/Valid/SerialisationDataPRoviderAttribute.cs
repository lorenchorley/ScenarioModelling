using ScenarioModel.Expressions.SemanticTree;
using System.Reflection;

namespace ScenarioModel.Tests.Valid;

public class SerialisationDataProviderAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => Enumerable.Empty<string>()
                     .Concat(ProgressiveContexts.GetContexts())
                     .Append(ValidScenario1.SerialisedContext)
                     .Select(s => new object[] { s });

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => data?[0]?.ToString() ?? "";

}