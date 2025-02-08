using Newtonsoft.Json;
using ProtoBuf;
using ScenarioModelling.Expressions.SemanticTree;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.Objects.SystemObjects.Interfaces;
using ScenarioModelling.References;
using YamlDotNet.Serialization;

namespace ScenarioModelling;

[ProtoContract]
public class System
{
    [ProtoMember(1)]
    public List<Entity> Entities { get; set; } = new();

    [ProtoMember(2)]
    public List<EntityType> EntityTypes { get; set; } = new();

    [ProtoMember(3)]
    public List<Aspect> Aspects { get; set; } = new();

    [ProtoMember(4)]
    public List<State> States { get; set; } = new();

    [ProtoMember(5)]
    public List<StateMachine> StateMachines { get; set; } = new();

    [ProtoMember(6)]
    public List<Relation> Relations { get; set; } = new();

    [ProtoMember(7)]
    public List<Transition> Transitions { get; set; } = new();

    [ProtoMember(8)]
    public List<Constraint> Constraints { get; set; } = new();

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<EntityReference> AllEntityReferences
        => Enumerable.Empty<EntityReference>()
                     .Concat(Aspects.Select(e => e.Entity.ReferenceOnly))
                     .Where(s => s != null)
                     .Cast<EntityReference>();

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<EntityTypeReference> AllEntityTypeReferences
        => Enumerable.Empty<EntityTypeReference>()
                     .Concat(Entities.Select(e => e.EntityType.ReferenceOnly))
                     .Where(s => s != null)
                     .Cast<EntityTypeReference>();

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<AspectReference> AllAspectReferences
        => Enumerable.Empty<AspectReference>()
                     .Concat(Entities.SelectMany(e => e.Aspects.AllReferencesOnly))
                     .Where(s => s != null)
                     .Cast<AspectReference>();

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<StateReference> AllStateReferences
        => AllStateful.Select(e => e.State.ReferenceOnly)
                      .Concat(StateMachines.SelectMany(sm => sm.States.AllReferencesOnly))
                      .Append(Transitions.Select(t => t.SourceState.ReferenceOnly))
                      .Append(Transitions.Select(t => t.DestinationState.ReferenceOnly))
                      .Where(s => s != null)
                      .Cast<StateReference>();

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<StateMachineReference> AllStateMachineReferences
        => Enumerable.Empty<StateMachineReference>()
                     .Concat(Aspects.Select(e => e.AspectType?.StateMachine.ReferenceOnly))
                     .Concat(EntityTypes.Select(e => e.StateMachine.ReferenceOnly))
                     .Where(s => s != null)
                     .Cast<StateMachineReference>();

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<RelationReference> AllRelationReferences
        => Enumerable.Empty<RelationReference>()
                     .Concat(AllRelatable.SelectMany(e => e.Relations.AllReferencesOnly))
                     .Where(s => s != null)
                     .Cast<RelationReference>();

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<TransitionReference> AllTransitionReferences
        => Enumerable.Empty<TransitionReference>()
                     .Concat(StateMachines.SelectMany(s => s.Transitions.AllReferencesOnly))
                     .Where(s => s != null)
                     .Cast<TransitionReference>();

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<IStateful> AllStateful
        => Enumerable.Empty<IStateful>()
                     .Concat(Entities)
                     .Concat(AllAspects)
                     .Concat(Relations);

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<IRelatable> AllRelatable
        => Enumerable.Empty<IRelatable>()
                     .Concat(Entities)
                     .Concat(Entities.SelectMany(x => x.Aspects));

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<Aspect> AllAspects
        => Enumerable.Empty<Aspect>()
                     .Concat(Entities.SelectMany(x => x.Aspects));

    [JsonIgnore]
    [YamlIgnore]
    public IEnumerable<ConstraintReference> AllConstraintReferences
        => Enumerable.Empty<ConstraintReference>();

    /// <summary>
    /// Not finished yet, not all cases covered !
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal object ResolveValue(CompositeValue value)
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
                return entity.State.ResolvedValue.Name;
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
