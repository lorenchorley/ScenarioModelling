using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.SystemObjects.Constraints.Traversal;

public interface IConstraintExpressionNode
{
    object Accept(IConstraintNodeVisitor visitor);
}
