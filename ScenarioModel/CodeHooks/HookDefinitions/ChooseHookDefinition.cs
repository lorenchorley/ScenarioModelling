using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.CodeHooks.HookDefinitions;

public delegate bool ChooseHook(bool result);

[NodeLike<INodeHookDefinition, ChooseNode>]
public class ChooseHookDefinition(string[] Choices) : INodeHookDefinition
{
    [NodeLikeProperty]
    public string? Id { get; private set; }

    [NodeLikeProperty]
    public List<(string nodeId, string choiceName)> Jumps { get; } = new();

    [NodeLikeProperty]
    public List<bool> RecordedChooseEvents { get; } = new();

    private bool ChooseHook(bool result)
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
        Id = id;
        return this;
    }

    public ChooseHookDefinition WithJump(string nodeId, string choiceName)
    {
        Jumps.Add((nodeId, choiceName));
        return this;
    }
}
