using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;

public interface IInSituNodeHookDefinition : INodeHookDefinition
{
    void BuildAndRegister();
}
