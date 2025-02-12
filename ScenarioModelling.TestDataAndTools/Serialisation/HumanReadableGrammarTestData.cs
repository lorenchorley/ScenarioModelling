using System.Diagnostics.CodeAnalysis;

namespace ScenarioModelling.TestDataAndTools.Serialisation;

public record HumanReadableGrammarTestData(string Name, string Expression, [StringSyntax(StringSyntaxAttribute.Regex)] string ExpectedResultRegex) { }
