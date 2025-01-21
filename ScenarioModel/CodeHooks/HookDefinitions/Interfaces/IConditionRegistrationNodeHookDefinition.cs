using ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;

public interface IConditionRegistrationNodeHookDefinition<TDefinition, TConditionHook> : INodeHookDefinition
    where TDefinition : IConditionRegistrationNodeHookDefinition<TDefinition, TConditionHook>
    where TConditionHook : Delegate
{
    public TDefinition GetConditionHook(out TConditionHook conditionHook);
    void Build();
}
