namespace ScenarioModel.CodeHooks.HookDefinitions.Interfaces;

public interface IHookDefinition
{
    void Validate();
    bool Validated { get; }
}
