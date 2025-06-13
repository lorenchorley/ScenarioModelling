using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CodeHooks.Utils;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.TestCaseNodes;

namespace ScenarioModelling.CodeHooks.HookDefinitions.TestCaseObjects;

[NodeLike<TestCase, ITestCaseHookDefinition, TestCase>]
public class TestCaseHookDefinition : ITestCaseHookDefinition
{
    private readonly IMetaStoryHookFunctions _hookFunctions;

    public bool Validated { get; private set; } = false;
    public TestCase Node { get; private set; }

    public TestCaseHookDefinition(string name, Context context, string metaStoryName, IMetaStoryHookFunctions hookFunctions)
    {
        Node = new TestCase(context, metaStoryName)
        {
            Name = name
        };

        _hookFunctions = hookFunctions;
    }

    public TestCase GetNode()
    {
        return Node;
    }

    public TestCaseHookDefinition AddInitialState(string statefulObjectName, string stateName)
    {
        Node.InitialStates.Add(statefulObjectName, stateName);

        return this;
    }
    
    public TestCaseHookDefinition AddFinalState(string statefulObjectName, string stateName)
    {
        Node.FinalStates.Add(statefulObjectName, stateName);

        return this;
    }

    public void Validate()
    {
        // TODO Verify that all the stateful objects and states exist in the meta state
        foreach (var statefulObjectName in Node.InitialStates.Keys.Concat(Node.FinalStates.Keys))
        {
            bool foundStatefulObject = new StateReference(Node.Context.MetaState) { Name = statefulObjectName }.IsResolvable();
            if (!foundStatefulObject)
                throw new Exception($"Stateful object {statefulObjectName} does not exist in the meta state");
        }
        
        foreach (var stateName in Node.InitialStates.Values.Concat(Node.FinalStates.Values))
        {
            if (!Node.Context.MetaState.States.Any(s => s.Name.IsEqv(stateName)))
                throw new Exception($"State {stateName} does not exist in the meta state");
        }
        
        Validated = true;
    }

    public void BuildAndRegister()
    {
        Validate();
        _hookFunctions.FinaliseDefinition(this);
    }
}
