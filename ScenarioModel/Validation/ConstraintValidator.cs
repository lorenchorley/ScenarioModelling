using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.Validation;

public class ConstraintValidator :  IValidator<ConstraintExpression>
{
    private readonly System _system;
    private readonly ConstraintValidatorVisitor _visitor;

    public ConstraintValidator(System system)
    {
        _system = system;
        _visitor = new ConstraintValidatorVisitor(system);
    }

    public ValidationErrors Validate(ConstraintExpression constraint)
    {
        constraint.Accept(_visitor);    
        
        return _visitor.Errors;
    }
}