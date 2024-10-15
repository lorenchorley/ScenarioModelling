using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public class ValueComposite : Value
{
    public List<string> ValueList { get; set; } = new();

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitValueComposite(this);
}
