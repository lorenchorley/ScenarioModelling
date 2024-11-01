using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.SystemObjects.Entities;
using ScenarioModel.Objects.SystemObjects.Relations;
using ScenarioModel.Objects.SystemObjects.States;

namespace ScenarioModel;

public class System
{
    public List<EntityType> EntityTypes { get; set; } = new();
    public List<AspectType> AspectTypes { get; set; } = new();
    public List<Entity> Entities { get; set; } = new();
    public List<StateMachine> StateMachines { get; set; } = new();
    public List<Expression> Constraints { get; set; } = new();
    public List<Relation> TopLevelRelations { get; set; } = new();

    public void Initialise()
    {
        foreach (var stateMachine in StateMachines)
        {
            foreach (var state in stateMachine.States)
            {
                state.StateMachine = stateMachine;

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
                         .DistinctByReference();
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
                         .Concat(TopLevelRelations)
                         .Concat(Entities.SelectMany(x => x.Relations))
                         .Concat(Entities.SelectMany(e => e.Aspects).SelectMany(a => a.Relations));
    }

    public IEnumerable<Aspect> AllAspects
    {
        get => Enumerable.Empty<Aspect>()
                         .Concat(Entities.SelectMany(x => x.Aspects));
    }

    public IEnumerable<IRelatable> AllRelatable
    {
        get => Enumerable.Empty<IRelatable>()
                         .Concat(Entities)
                         .Concat(Entities.SelectMany(x => x.Aspects));
    }

    public bool HasState(string stateName)
    {
        return AllStates.Any(s => s.Name.IsEqv(stateName));
    }

    /// <summary>
    /// Not finished yet, not all cases covered !
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal object ResolveValue(ValueComposite value)
    {
        if (value.ValueList.Count == 0)
        {
            throw new Exception("Value cannot be empty");
        }

        string first = value.ValueList[0];

        Entity? entity = Entities.FirstOrDefault(e => e.Name.IsEqv(first));

        if (entity != null)
        {
            if (value.ValueList.Count == 1)
            {
                return entity;
            }

            if (value.ValueList[1].IsEqv("State"))
            {
                return entity.State.Name;
            }

            // TODO aspects and other cases
        }

        // TODO other cases

        if (value.ValueList.Count == 1)
        {
            // we can suppose it's just a string
            return value.ValueList[0];
        }

        throw new Exception("Unsupported value");
    }
}
