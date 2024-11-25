﻿using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Validation;

namespace ScenarioModel.ContextValidation.SystemValidation;

[ObjectLike<IObjectValidator, Transition>]
public class TransitionValidator : IObjectValidator<Transition>
{
    public ValidationErrors Validate(System system, Transition transition)
    {
        ValidationErrors validationErrors = new();


        return validationErrors;
    }
}