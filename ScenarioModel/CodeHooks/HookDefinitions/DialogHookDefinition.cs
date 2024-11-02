using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.CodeHooks.HookDefinitions;

[NodeLike<INodeHookDefinition, DialogNode>]
public class DialogHookDefinition(string Text) : INodeHookDefinition
{
    [NodeLikeProperty]
    public string Character { get; private set; }

    public DialogHookDefinition SetCharacter(string character)
    {
        Character = character;
        return this;
    }
}
