using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, DialogNode>]
public class DialogHookDefinition : INodeHookDefinition
{
    private readonly Action _finaliseDefinition;

    public bool Validated { get; private set; } = false;
    public DialogNode Node { get; private set; }
    public DefinitionScope Scope { get; }
    public DefinitionScopeSnapshot ScopeSnapshot { get; }

    public DialogHookDefinition(DefinitionScope scope, string text, Action finaliseDefinition)
    {
        Node = new DialogNode()
        {
            TextTemplate = text
        };
        Scope = scope;
        _finaliseDefinition = finaliseDefinition;
        ScopeSnapshot = Scope.TakeSnapshot();
    }

    public IScenarioNode GetNode()
    {
        return Node;
    }

    public DialogHookDefinition SetCharacter(string character)
    {
        Node.Character = character;
        return this;
    }

    public DialogHookDefinition SetId(string id)
    {
        Node.Name = id;
        return this;
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

    public void ReplaceNodeWithExisting(IScenarioNode preexistingNode)
    {
        if (preexistingNode is not DialogNode node)
            throw new Exception($"When trying to replace the hook definition's generated node with a preexisting node, the types did not match (preexisting type : {preexistingNode.GetType().Name}, generated type : {Node.GetType().Name})");

        Node = node;
    }
}
