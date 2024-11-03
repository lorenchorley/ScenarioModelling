using ScenarioModel.Objects.ScenarioObjects.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions;

public interface INodeHookDefinition
{
    IScenarioNode GetNode();
}
