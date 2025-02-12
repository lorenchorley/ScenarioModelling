using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References.GeneralisedReferences;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

[StoryNodeLike<INodeHookDefinition, TransitionNode>]
public class TransitionHookDefinition : IInSituNodeHookDefinition
{
    private readonly IHookFunctions _hookFunctions;

    public bool Validated { get; private set; } = false;
    public TransitionNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public TransitionHookDefinition(DefinitionScope scope, MetaState System, string StatefulObjectName, string Transition, IHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();

        Node = new TransitionNode()
        {
            StatefulObject = new StatefulObjectReference(System) { Name = StatefulObjectName },
            TransitionName = Transition
        };
    }

    public IStoryNode GetNode()
    {
        return Node;
    }

    public TransitionHookDefinition SetId(string id)
    {
        Node.Name = id;
        return this;
    }

    public void Validate()
    {
        Validated = true;
    }

    public void BuildAndRegister()
    {
        Validate();
        _hookFunctions.FinaliseDefinition(this);
        _hookFunctions.RegisterEventForHook(this, _ => { });
    }

    public void ReplaceNodeWithExisting(IStoryNode preexistingNode)
    {
        if (preexistingNode is not TransitionNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
