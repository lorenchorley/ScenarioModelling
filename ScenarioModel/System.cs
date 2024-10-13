using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel;

public class System
{
    public List<EntityType> EntityTypes { get; set; } = new();
    public List<AspectType> AspectTypes { get; set; } = new();
    public List<Entity> Entities { get; set; } = new();
    public List<StateType> StateMachines { get; set; } = new();
    public List<ConstraintExpression> Constraints { get; set; } = new();

    public void Initialise()
    {
        foreach (var stateType in StateMachines)
        {
            foreach (var state in stateType.States)
            {
                state.StateType = stateType;
            }
        }
    }

    public IEnumerable<State> AllStates
    {
        get => StateMachines.SelectMany(x => x.States);
    }

    public IEnumerable<Relation> AllRelations
    {
        get => Enumerable.Empty<Relation>()
                         .Concat(Entities.SelectMany(x => x.Relations))
                         .Concat(Entities.SelectMany(e => e.Aspects).SelectMany(a => a.Relations));
    }
    
    public IEnumerable<Aspect> AllAspects
    {
        get => Enumerable.Empty<Aspect>()
                         .Concat(Entities.SelectMany(x => x.Aspects));
    }

    public bool HasState(string stateName)
    {
        return AllStates.Any(s => string.Equals(s.Name, stateName));
    }
}
