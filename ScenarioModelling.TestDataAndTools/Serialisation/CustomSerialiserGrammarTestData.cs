using System.Diagnostics.CodeAnalysis;

namespace ScenarioModelling.TestDataAndTools.Serialisation;

public record CustomSerialiserGrammarTestData(string Name, string Expression, [StringSyntax(StringSyntaxAttribute.Regex)] string ExpectedResultRegex) { }
