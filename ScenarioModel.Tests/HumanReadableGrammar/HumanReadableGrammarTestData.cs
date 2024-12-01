using System.Diagnostics.CodeAnalysis;

namespace ScenarioModel.Tests.HumanReadableGrammar;

public record HumanReadableGrammarTestData(string Name, string Expression, [StringSyntax(StringSyntaxAttribute.Regex)] string ExpectedResultRegex) { }
