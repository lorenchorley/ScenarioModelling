using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, DialogNode>]
public class DialogHookDefinition : INodeHookDefinition
{
    public bool Validated { get; private set; } = false;
    public DialogNode Node { get; private set; }
    public DefinitionScope CurrentScope { get; }

    public DialogHookDefinition(DefinitionScope currentScope, string text)
    {
        Node = new DialogNode()
        {
            TextTemplate = text
        };
        CurrentScope = currentScope;
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
}
