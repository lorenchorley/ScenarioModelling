using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;

[NodeLike<INodeHookDefinition, DialogNode>]
public class DialogHookDefinition(string Text) : INodeHookDefinition
{
    [NodeLikeProperty]
    public string Character { get; private set; }

    public IScenarioNode GetNode()
    {
        return new DialogNode()
        {
            TextTemplate = Text,
            Character = Character
        };
    }

    public DialogHookDefinition SetCharacter(string character)
    {
        Character = character;
        return this;
    }
}
