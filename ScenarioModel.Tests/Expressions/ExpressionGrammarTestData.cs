using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace ScenarioModel.Tests;

public record ExpressionGrammarTestData(string Name, string Expression, [StringSyntax(StringSyntaxAttribute.Regex)] string ExpectedResultRegex) { }