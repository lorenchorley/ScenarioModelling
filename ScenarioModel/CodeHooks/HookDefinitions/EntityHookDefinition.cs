namespace ScenarioModel.CodeHooks.HookDefinitions;

public class EntityHookDefinition(string Name)
{
    public string? State { get; private set; }

    public EntityHookDefinition SetState(string state)
    {
        State = state;
        return this;
    }
}
