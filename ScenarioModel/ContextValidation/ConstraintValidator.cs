using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Expressions.Validation;

namespace ScenarioModel.Validation;

public class ConstraintValidator : IValidator<Expression>
{
    private readonly System _system;
    private readonly ExpressionValidator _visitor;

    public ConstraintValidator(System system)
    {
        _system = system;
        _visitor = new ExpressionValidator(system);
    }

    public ValidationErrors Validate(Expression constraint)
    {
        constraint.Accept(_visitor);

        return _visitor.Errors;
    }
}