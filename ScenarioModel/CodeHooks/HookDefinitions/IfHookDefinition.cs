using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.CodeHooks.HookDefinitions;

public delegate bool IfHook(bool result);

[NodeLike<INodeHookDefinition, IfNode>]
public class IfHookDefinition(string Name) : INodeHookDefinition
{
    [NodeLikeProperty]
    public List<bool> RecordedIfEvents { get; } = new();

    private bool IfHook(bool result)
    {
        RecordedIfEvents.Add(result);
        return result;
    }

    public IfHookDefinition GetConditionHook(out IfHook ifHook)
    {
        ifHook = IfHook;
        return this;
    }
}
