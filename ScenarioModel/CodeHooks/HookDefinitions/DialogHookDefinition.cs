using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;

namespace ScenarioModel.CodeHooks.HookDefinitions;

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
