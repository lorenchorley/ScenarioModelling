using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;

public interface INodeHookDefinition : IHookDefinition
{
    IStoryNode GetNode();

    /// <summary>
    /// This allows for the replacement of the node created by this hook by a preexisting node in the graph so as not to duplicate the graph nodes.
    /// </summary>
    /// <param name="preexistingNode"></param>
    void ReplaceNodeWithExisting(IStoryNode preexistingNode);
    DefinitionScope Scope { get; }
    DefinitionScopeSnapshot ScopeSnapshot { get; }
}
