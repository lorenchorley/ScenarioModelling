using ScenarioModel.SystemObjects.Constraints.Traversal;
using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Entities;

public abstract class ConstraintExpression : IConstraintExpressionNode
{
    public abstract object Accept(IConstraintNodeVisitor visitor);
}
