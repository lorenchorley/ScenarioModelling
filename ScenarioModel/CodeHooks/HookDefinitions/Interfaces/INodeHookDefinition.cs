using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;

public interface INodeHookDefinition : IHookDefinition
{
    IScenarioNode GetNode();
    void ReplaceNodeWithExisting(IScenarioNode preexistingNode);
    DefinitionScope Scope { get; }
    DefinitionScopeSnapshot ScopeSnapshot { get; }
    void Build();
}
