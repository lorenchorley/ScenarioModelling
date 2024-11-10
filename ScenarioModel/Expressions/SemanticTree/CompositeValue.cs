using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.SemanticTree;

public record CompositeValue : Value
{
    public List<string> ValueList { get; set; } = new();

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitCompositeValue(this);

    override public string ToString()
        => "CompositeValue { " + ValueList.CommaSeparatedList() + " }";
}
