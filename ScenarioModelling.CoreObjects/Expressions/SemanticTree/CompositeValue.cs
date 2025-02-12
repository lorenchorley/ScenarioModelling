using ScenarioModelling.CoreObjects.Expressions.Traversal;

namespace ScenarioModelling.CoreObjects.Expressions.SemanticTree;

public record CompositeValue : Value
{
    public List<string> ValueList { get; set; } = new();

    public override object Accept(IExpressionVisitor visitor)
        => visitor.VisitCompositeValue(this);

    override public string ToString()
        => "CompositeValue { " + ValueList.CommaSeparatedList() + " }";
}
