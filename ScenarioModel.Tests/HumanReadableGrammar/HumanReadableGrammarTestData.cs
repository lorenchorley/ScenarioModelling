using System.Diagnostics.CodeAnalysis;

namespace ScenarioModelling.Tests.HumanReadableGrammar;

public record HumanReadableGrammarTestData(string Name, string Expression, [StringSyntax(StringSyntaxAttribute.Regex)] string ExpectedResultRegex) { }
