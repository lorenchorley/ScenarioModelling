using ScenarioModelling.Execution;
using ScenarioModelling.Mocks.Utils;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Mocks;

public class MockCallResult
{
    public Dictionary<string, string> FinalStatesByStatefulObjectName { get; private set; }
    public Story Story { get; private set; }

    internal BusinessToStateMap BusinessObjectToStateMap { get; set; }

    public MockCallResult(Dictionary<string, string> finalStatesByEntityName, Story story)
    {
        FinalStatesByStatefulObjectName = finalStatesByEntityName;
        Story = story;
    }

    public string GetStateOf(string entityName)
    {
        if (FinalStatesByStatefulObjectName.TryGetValue(entityName, out string state))
        {
            return state;
        }

        var entities = FinalStatesByStatefulObjectName.Keys.CommaSeparatedList();
        throw new MetaStoryMockException($"Entity '{entityName}' not found in mock call result. The entities present is the result were the following : {entities}");
    }

    public T GetValue<T>(string businessType)
    {
        string statefulObjectName = BusinessObjectToStateMap.GetStatefulObjectName(businessType);

        string stateName = GetStateOf(statefulObjectName);

        return BusinessObjectToStateMap.GetValue<T>(businessType, stateName);
    }
}
