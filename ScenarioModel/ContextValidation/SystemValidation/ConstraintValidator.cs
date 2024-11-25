﻿using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Expressions.Validation;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Validation;

namespace ScenarioModel.ContextValidation.SystemValidation;

[ObjectLike<IObjectValidator, Constraint>]
public class ConstraintValidator : IObjectValidator<Constraint>
{
    public ValidationErrors Validate(System system, Constraint constraint)
    {
        ExpressionInitialiser _visitor = new ExpressionInitialiser(system);
        constraint.Condition.Accept(_visitor);
        return _visitor.Errors;
    }
}