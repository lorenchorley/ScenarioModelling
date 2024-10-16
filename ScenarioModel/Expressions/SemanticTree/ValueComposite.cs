using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record ValueComposite : Value
{
    public List<string> ValueList { get; set; } = new();

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitValueComposite(this);

    override public string ToString()
        => "ValueComposite { " + string.Join(", ", ValueList) + " }";
}
