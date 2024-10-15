using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace ScenarioModel.Tests;

public record HumanReadableGrammarTestData(string Name, string Expression, [StringSyntax(StringSyntaxAttribute.Regex)] string ExpectedResultRegex) { }
