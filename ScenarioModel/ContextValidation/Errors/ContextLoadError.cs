﻿namespace ScenarioModel.ContextValidation.Errors;

public class ContextLoadError(string message) : ValidationError(message)
{

}