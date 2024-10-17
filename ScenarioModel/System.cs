using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.Relations;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel;

public class System
{
    public List<EntityType> EntityTypes { get; set; } = new();
    public List<AspectType> AspectTypes { get; set; } = new();
    public List<Entity> Entities { get; set; } = new();
    public List<StateMachine> StateMachines { get; set; } = new();
    public List<Expression> Constraints { get; set; } = new();

    public void Initialise()
    {
        foreach (var stateType in StateMachines)
        {
            foreach (var state in stateType.States)
            {
                state.StateMachine = stateType;

            }
        }
    }

    public IEnumerable<State> AllStates
    {
        get => Enumerable.Empty<State?>()
                         .Concat(Entities.Select(e => e.State))
                         .Concat(AllAspects.Select(e => e.State))
                         .Concat(AllRelations.Select(e => e.State))
                         .Concat(StateMachines.SelectMany(x => x.States))
                         .Where(s => s != null)
                         .Cast<State>()
                         .Distinct();
    }

    public IEnumerable<IStateful> AllStateful
    {
        get => Enumerable.Empty<IStateful>()
                         .Concat(Entities)
                         .Concat(AllAspects)
                         .Concat(AllRelations);
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
        return AllStates.Any(s => s.Name.IsEqv(stateName));
    }
}
