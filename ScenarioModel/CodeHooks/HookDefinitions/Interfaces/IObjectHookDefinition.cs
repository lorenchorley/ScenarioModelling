namespace ScenarioModel.CodeHooks.HookDefinitions.Interfaces;

public enum HookExecutionMode
{
    Simulation,
    FirstScenarioConstruction,
    SubsequentScenarioConstruction
}

public interface IObjectHookDefinition
{
    HookExecutionMode HookExecutionMode { get; }
}
