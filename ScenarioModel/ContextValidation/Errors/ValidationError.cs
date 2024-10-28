﻿namespace ScenarioModel.Validation;

public class ValidationError(string message)
{
    public string Message { get; } = message;

    public override string ToString()
    {
        return GetType().Name + " : " + Message;
    }
}