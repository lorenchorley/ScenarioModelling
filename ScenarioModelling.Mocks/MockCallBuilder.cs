using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.SystemObjects;
using ScenarioModelling.Execution;
using ScenarioModelling.Mocks.Utils;

namespace ScenarioModelling.Mocks;

public class MockCallBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, string> _stateNamesByStatefulObjectName = new();

    public string TargetMetaStoryName { get; internal set; } = string.Empty;
    internal BusinessToStateMap BusinessObjectToStateMap { get; set; }

    public MockCallBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    internal MockCallBuilder SetValue<TBusinessValue>(string businessType, TBusinessValue businessValue) where TBusinessValue : notnull
    {
        string statefulObjectName = BusinessObjectToStateMap.GetStatefulObjectName(businessType);
        string stateName = BusinessObjectToStateMap.GetStateName(businessType, businessValue);

        return SetState(statefulObjectName, stateName);
    }

    internal MockCallBuilder SetState(string statefulObjectName, string stateName)
    {
        _stateNamesByStatefulObjectName.Add(statefulObjectName, stateName);

        return this;
    }

    internal MockCallResult Call()
    {
        // Do the simulation
        MockStoryRunner runner = _serviceProvider.GetRequiredService<MockStoryRunner>();

        runner.Executor.SetBeforeInitStory(Context =>
        {
            foreach (var pair in _stateNamesByStatefulObjectName)
            {
                var reference = new StateReference(Context.MetaState)
                {
                    Name = pair.Value
                };
                Entity entity = Context.MetaState.Entities.FirstOrDefault(e => e.Name.IsEqv(pair.Key));
                entity.State.SetReference(reference);
            }
        });

        Story story = runner.Run(TargetMetaStoryName);

        Dictionary<string, string> finalStatesByEntityName = runner.GetFinalStatesByEntityName();

        return new MockCallResult(finalStatesByEntityName, story)
        {
            BusinessObjectToStateMap = BusinessObjectToStateMap
        };
    }
}
