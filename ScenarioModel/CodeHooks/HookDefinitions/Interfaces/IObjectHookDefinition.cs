namespace ScenarioModel.CodeHooks.HookDefinitions.Interfaces;

public enum HookExecutionMode
{
    Simulation,
    FirstScenarioConstruction,
    SubsequentScenarioConstruction
}

public interface IObjectHookDefinition : IHookDefinition
{
    HookExecutionMode HookExecutionMode { get; }
}
