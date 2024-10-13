﻿namespace ScenarioModel.SystemObjects.States;

public class State : INameful
{
    public string Name { get; set; } = "";
    public StateType StateType { get; set; } = null!;
    public List<string> Transitions { get; set; } = new();
}
