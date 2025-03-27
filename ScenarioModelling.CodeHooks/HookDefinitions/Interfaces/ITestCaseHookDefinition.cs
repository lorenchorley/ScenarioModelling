using ScenarioModelling.CoreObjects.TestCaseNodes;

namespace ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;

public interface ITestCaseHookDefinition : IHookDefinition
{
    TestCase Node { get; }
}
