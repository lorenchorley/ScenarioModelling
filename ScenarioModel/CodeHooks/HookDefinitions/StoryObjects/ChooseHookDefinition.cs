using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

public delegate string ChooseHook(string result);

[StoryNodeLike<INodeHookDefinition, ChooseNode>]
public class ChooseHookDefinition : INodeHookDefinition
{
    private readonly Action _finaliseDefinition;

    [StoryNodeLikeProperty]
    public List<string> RecordedChooseEvents { get; } = new();

    public bool Validated { get; private set; } = false;
    public ChooseNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public ChooseHookDefinition(DefinitionScope scope, Action finaliseDefinition)
    {
        Node = new ChooseNode();
        Scope = scope;
        _finaliseDefinition = finaliseDefinition;
        ScopeSnapshot = Scope.TakeSnapshot();
    }

    private string ChooseHook(string result)
    {
        RecordedChooseEvents.Add(result);
        return result;
    }

    public ChooseHookDefinition GetConditionHook(out ChooseHook chooseCondition)
    {
        chooseCondition = ChooseHook;
        return this;
    }

    public ChooseHookDefinition SetId(string id)
    {
        Node.Name = id;
        return this;
    }

    public ChooseHookDefinition WithJump(string nodeId, string choiceName)
    {
        Node.Choices.Add((nodeId, choiceName));
        return this;
    }

    public ChooseHookDefinition SetAsImplicit()
    {
        Node.Implicit = true;
        return this;
    }

    public IStoryNode GetNode()
    {
        return Node;
    }

    public void Validate()
    {
        Validated = true;
    }

    public void Build()
    {
        Validate();
        _finaliseDefinition();
    }

    public void ReplaceNodeWithExisting(IStoryNode preexistingNode)
    {
        if (preexistingNode is not ChooseNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
