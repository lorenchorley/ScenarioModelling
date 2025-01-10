using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.Interfaces;

public interface INodeHookDefinition : IHookDefinition
{
    IScenarioNode GetNode();
    DefinitionScope CurrentScope { get; }
}
