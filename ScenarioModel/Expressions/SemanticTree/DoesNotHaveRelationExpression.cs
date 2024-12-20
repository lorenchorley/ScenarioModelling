﻿using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record DoesNotHaveRelationExpression : Expression
{
    public string? Name { get; set; }
    public CompositeValue Left { get; set; } = null!;
    public CompositeValue Right { get; set; } = null!;

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitDoesNotHaveRelation(this);
}
