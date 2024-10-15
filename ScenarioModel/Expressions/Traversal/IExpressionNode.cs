using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Expressions.Traversal;

public interface IExpressionNode
{
    object Accept(IExpressionVisitor visitor);
}
