using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel;

public class System
{
    public List<EntityType> EntityTypes { get; set; } = new();
    public List<Entity> Entities { get; set; } = new();
    public List<StateType> StateTypes { get; set; } = new();

    public void Initialise()
    {
        foreach (var stateType in StateTypes)
        {
            foreach (var state in stateType.States)        
            {
                state.StateType = stateType;
            }
        }
    }

    public IEnumerable<State> States
    {
        get => StateTypes.SelectMany(x => x.States);
    }

    public bool HasState(string stateName)
    {
        return States.Any(s => string.Equals(s.Name, stateName));
    }
}
