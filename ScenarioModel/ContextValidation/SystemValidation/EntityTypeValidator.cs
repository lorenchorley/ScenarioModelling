﻿using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.SystemObjects;

namespace ScenarioModel.ContextValidation.SystemValidation;

[ObjectLike<IObjectValidator, EntityType>]
public class EntityTypeValidator : IObjectValidator<EntityType>
{
    public ValidationErrors Validate(System system, EntityType entity)
    {
        ValidationErrors validationErrors = new();
        // Initial state is of type StateMachine

        return validationErrors;
    }
}