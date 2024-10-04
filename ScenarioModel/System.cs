using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel;

public class System
{
    public List<EntityType> EntityTypes { get; set; } = new();
    public List<Entity> Entities { get; set; } = new();
    public List<StateType> StateTypes { get; set; } = new();
    public List<ConstraintExpression> Constraints { get; set; } = new();

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

    public IEnumerable<Relation> Relations
    {
        get => Enumerable.Empty<Relation>()
                .Concat(Entities.SelectMany(x => x.Relations))
                .Concat(Entities.SelectMany(e => e.Aspects).SelectMany(a => a.Relations));
    }

    public bool HasState(string stateName)
    {
        return States.Any(s => string.Equals(s.Name, stateName));
    }
}
