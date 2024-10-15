using ScenarioModel.Execution.Events;
using ScenarioModel.Execution.Events.Factory;

namespace ScenarioModel.Execution.Analysis;

public class AnalysisFactory : IEventFactory
{
    public IScenarioEvent CreateEvent(string eventTypeName, Action configure)
    {
        throw new NotImplementedException();
    }
}
