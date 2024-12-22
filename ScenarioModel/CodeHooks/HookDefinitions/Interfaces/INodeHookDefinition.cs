using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.Interfaces;

public interface INodeHookDefinition
{
    IScenarioNode GetNode();
    void ValidateFinalState();
}
