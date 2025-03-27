using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.TestCaseNodes;

namespace ScenarioModelling.CodeHooks.HookDefinitions.TestCaseObjects;

[NodeLike<TestCase, ITestCaseHookDefinition, TestCase>]
public class TestCaseHookDefinition : ITestCaseHookDefinition
{
    public bool Validated { get; private set; } = false;
    public TestCase Node { get; private set; }

    private Dictionary<string, string> InitialStates { get; } = new();
    private Dictionary<string, string> ExpectedStates { get; } = new();

    public TestCaseHookDefinition(string name, Context context, string metaStoryName)
    {
        Node = new TestCase(context, metaStoryName)
        {
            Name = name
        };
    }

    public TestCase GetNode()
    {
        return Node;
    }

    public TestCaseHookDefinition AddInitialState(string statefulObjectName, string stateName)
    {
        InitialStates.Add(statefulObjectName, stateName);

        return this;
    }
    
    public TestCaseHookDefinition AddFinalState(string statefulObjectName, string stateName)
    {
        ExpectedStates.Add(statefulObjectName, stateName);

        return this;
    }

    public void Validate()
    {
        // TODO Verify that all the stateful objects and states exist in the meta state
        foreach (var statefulObjectName in InitialStates.Keys.Concat(ExpectedStates.Keys))
        {
            if (!Node.Context.MetaState.AllStateful.Any(s => s.Name.IsEqv(statefulObjectName)))
                throw new Exception($"Stateful object {statefulObjectName} does not exist in the meta state");
        }
        
        foreach (var stateName in InitialStates.Values.Concat(ExpectedStates.Values))
        {
            if (!Node.Context.MetaState.States.Any(s => s.Name.IsEqv(stateName)))
                throw new Exception($"State {stateName} does not exist in the meta state");
        }
        
        Validated = true;
    }

    public void BuildAndRegister()
    {
        Validate();
    }

}
