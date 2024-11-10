using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions;

public interface INodeHookDefinition
{
    IScenarioNode GetNode();
}
