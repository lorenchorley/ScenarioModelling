﻿using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record OrExpression : Expression
{
    public Expression Left { get; set; } = null!;
    public Expression Right { get; set; } = null!;

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitOr(this);
}
