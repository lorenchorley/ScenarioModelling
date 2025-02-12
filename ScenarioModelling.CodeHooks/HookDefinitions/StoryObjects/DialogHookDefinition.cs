using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

[StoryNodeLike<INodeHookDefinition, DialogNode>]
public class DialogHookDefinition : IInSituNodeHookDefinition
{
    private readonly IHookFunctions _hookFunctions;

    public bool Validated { get; private set; } = false;
    public DialogNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public DialogHookDefinition(DefinitionScope scope, string text, IHookFunctions hookFunctions)
    {
        _hookFunctions = hookFunctions;

        Node = new DialogNode()
        {
            TextTemplate = text
        };
        Scope = scope;
        ScopeSnapshot = Scope.TakeSnapshot();
    }

    public IStoryNode GetNode()
    {
        return Node;
    }

    public DialogHookDefinition WithCharacter(string character)
    {
        Node.Character = character;
        return this;
    }

    public DialogHookDefinition WithId(string id)
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
        if (preexistingNode is not DialogNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
