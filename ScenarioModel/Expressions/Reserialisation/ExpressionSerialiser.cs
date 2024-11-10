using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Expressions.Traversal;

namespace ScenarioModel.Expressions.Validation;

public class ExpressionSerialiser : IExpressionVisitor
{
    private readonly System _system;

    public ExpressionSerialiser(System system)
    {
        _system = system;
    }

    public object VisitAnd(AndExpression exp)
    {
        var leftResult = (string)exp.Left.Accept(this);
        var rightResult = (string)exp.Right.Accept(this);
        return $"{leftResult} AND {rightResult}";
    }

    public object VisitOr(OrExpression exp)
    {
        var leftResult = (string)exp.Left.Accept(this);
        var rightResult = (string)exp.Right.Accept(this);
        return $"{leftResult} OR {rightResult}";
    }

    public object VisitCompositeValue(CompositeValue value)
    {
        return string.Join(".", value.ValueList.Select(AddQuotesIfNecessary));
    }

    public object VisitEmpty(EmptyExpression exp)
    {
        return "";
    }

    public object VisitDoesNotHaveRelation(DoesNotHaveRelationExpression exp)
    {
        var leftResult = (string)exp.Left.Accept(this);
        var rightResult = (string)exp.Right.Accept(this);

        if (string.IsNullOrEmpty(exp.Name))
        {
            return $"{leftResult} -!> {rightResult}";
        }
        else
        {
            return $"{leftResult} -!> {rightResult} : {AddQuotesIfNecessary(exp.Name)}";
        }
    }

    public object VisitHasRelation(HasRelationExpression exp)
    {
        var leftResult = (string)exp.Left.Accept(this);
        var rightResult = (string)exp.Right.Accept(this);

        if (string.IsNullOrEmpty(exp.Name))
        {
            return $"{leftResult} -?> {rightResult}";
        }
        else
        {
            return $"{leftResult} -?> {rightResult} : {AddQuotesIfNecessary(exp.Name)}";
        }
    }

    public object VisitArgumentList(ArgumentList list)
    {
        return list.ExpressionList.Select(a => (string)a.Accept(this)).CommaSeparatedList();
    }

    public object VisitFunction(FunctionExpression exp)
    {
        var arguments = string.Join(", ", exp.Arguments.Accept(this));
        return $"{exp.Name}({arguments})";
    }

    public object VisitNotEqual(NotEqualExpression exp)
    {
        var leftResult = (string)exp.Left.Accept(this);
        var rightResult = (string)exp.Right.Accept(this);
        return $"{leftResult} != {rightResult}";
    }

    public object VisitEqual(EqualExpression exp)
    {
        var leftResult = (string)exp.Left.Accept(this);
        var rightResult = (string)exp.Right.Accept(this);
        return $"{leftResult} == {rightResult}";
    }

    public object VisitErroneousExpression(ErroneousExpression exp)
    {
        throw new NotImplementedException();
    }

    public object VisitBrackets(BracketsExpression bracketsExpression)
    {
        return $"({bracketsExpression.Expression.Accept(this)})";
    }

    private string AddQuotesIfNecessary(string str)
    {
        if (str.Contains(' '))
        {
            return $"\"{str}\"";
        }
        return str;
    }

}
