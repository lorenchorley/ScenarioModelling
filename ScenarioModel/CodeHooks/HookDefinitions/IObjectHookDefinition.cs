namespace ScenarioModel.CodeHooks.HookDefinitions;

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
