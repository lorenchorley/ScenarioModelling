namespace ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;

public enum HookExecutionMode
{
    Simulation,
    FirstMetaStoryConstruction,
    SubsequentMetaStoryConstruction
}

public interface IObjectHookDefinition : IHookDefinition
{
    HookExecutionMode HookExecutionMode { get; }
}
