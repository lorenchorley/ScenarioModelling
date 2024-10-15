using System.Reflection;

namespace ScenarioModel.Tests;

public class ExpressionGrammarTestDataProviderAttribute : Attribute, ITestDataSource
{
    public List<ExpressionGrammarTestData> TestData = new()
    {
        new(
            "Empty",
            "",
            """
            
            """
        ),
        new(
            "One object",
            "E1",
            """
            
            """
        ),
        new(
            "Subproperty of object",
            "E1.A1",
            """
            
            """
        ),
        new(
            "Subsubproperty of object",
            "E1.A1.State",
            """
            
            """
        ),
        new(
            "A string",
            @"""A string""",
            """
            
            """
        ),
    };

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        => TestData.Select(t => new object[] { t.Name, t.Expression, t.ExpectedResultRegex });

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => data?[0]?.ToString() ?? "";

}