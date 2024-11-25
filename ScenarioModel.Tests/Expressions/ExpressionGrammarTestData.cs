using ScenarioModel.Expressions.Evaluation;
using System.Diagnostics.CodeAnalysis;

namespace ScenarioModel.Tests;

public record ExpressionGrammarTestData(string Name, string Expression, [StringSyntax(StringSyntaxAttribute.Regex)] string Expected, bool IsValid, object? ExpectedEvaluatedValue = null, Type? ExpectedReturnType = null) { }
