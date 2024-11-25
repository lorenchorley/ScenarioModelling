﻿using ScenarioModel.Validation;

namespace ScenarioModel.ContextValidation.Interfaces;

public interface IObjectValidator
{

}

public interface IObjectValidator<T> : IObjectValidator
{
    ValidationErrors Validate(System system, T instance);
}