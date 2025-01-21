using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;

public interface INodeHookDefinition : IHookDefinition
{
    IStoryNode GetNode();
    void ReplaceNodeWithExisting(IStoryNode preexistingNode);
    DefinitionScope Scope { get; }
    DefinitionScopeSnapshot ScopeSnapshot { get; }
}
