using System.Diagnostics.CodeAnalysis;

namespace ScenarioModelling.Tests.Expressions;

public record ExpressionGrammarTestData(string Name, string Expression, [StringSyntax(StringSyntaxAttribute.Regex)] string Expected, bool IsValid, object? ExpectedEvaluatedValue = null, Type? ExpectedReturnType = null) { }
