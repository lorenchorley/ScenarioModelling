using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.Interfaces;

public interface INodeHookDefinition : IHookDefinition
{
    IScenarioNode GetNode();
    void ReplaceNodeWithExisting(IScenarioNode preexistingNode);
    DefinitionScope Scope { get; }
    DefinitionScopeSnapshot ScopeSnapshot { get; }
    void Build();
}
