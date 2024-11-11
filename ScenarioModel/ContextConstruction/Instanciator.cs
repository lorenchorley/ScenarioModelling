using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

using Relation = ScenarioModel.Objects.SystemObjects.Relation;

namespace ScenarioModel.ContextConstruction;

public class Instanciator(System System)
{
    private Dictionary<Type, int> _countersByType = new();

    /// <summary>
    /// GetOrNew works by either finding an existing instance in the provided system via a reference, or creating a new instance directly and registering it.
    /// </summary>
    /// <typeparam name="TVal"></typeparam>
    /// <typeparam name="TRef"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public TVal GetOrNew<TVal, TRef>(string? name = null, Definition? definition = null)
        where TVal : IIdentifiable
        where TRef : IReference<TVal>
    {
        name = name ?? TryGetNameFromDefinition(definition);

        return NewReference<TVal, TRef>(name)
            .ResolveReference()
            .Match(
                Some: e => e,
                None: () => New<TVal>(name: name, definition: definition)
            );
    }

    /// <summary>
    /// New creates a new instance of the provided type, and registers it in the provided system.
    /// </summary>
    /// <typeparam name="TVal"></typeparam>
    /// <param name="name"></param>
    /// <param name="definition"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public TVal New<TVal>(string? name = null, Definition? definition = null)
        where TVal : IIdentifiable
    {
        object instance = typeof(TVal).Name switch // TODO Exhaustivity ?
        {
            nameof(Entity) => new Entity(System),
            nameof(EntityType) => new EntityType(System),
            nameof(Aspect) => new Aspect(System),
            nameof(State) => new State(System),
            nameof(StateMachine) => new StateMachine(System),
            nameof(Transition) => new Transition(System),
            nameof(Relation) => new Relation(System),
            nameof(Constraint) => new Constraint(System),
            nameof(Scenario) => new Scenario() { System = System },
            _ => throw new NotImplementedException($"Reference type {typeof(TVal).Name} not implemented.")
        };

        return Name<TVal, TVal>((TVal)instance, name, definition);
    }

    public TRef NewReference<TVal, TRef>(string? name = null, Definition? definition = null)
        where TVal : IIdentifiable
        where TRef : IReference<TVal>
    {
        object reference = typeof(TRef).Name switch // TODO Exhaustivity ?
        {
            nameof(EntityReference) => new EntityReference(System),
            nameof(EntityTypeReference) => new EntityTypeReference(System),
            nameof(AspectReference) => new AspectReference(System),
            nameof(StateReference) => new StateReference(System),
            nameof(StateMachineReference) => new StateMachineReference(System),
            nameof(TransitionReference) => new TransitionReference(System),
            nameof(RelationReference) => new RelationReference(System),
            nameof(ConstraintReference) => new ConstraintReference(System),
            _ => throw new NotImplementedException($"Reference type {typeof(TRef).Name} not implemented.")
        };

        return Name<TRef, TVal>((TRef)reference, name, definition);
    }

    public TVal Name<TVal, TKey>(TVal obj, string? name = null, Definition? def = null)
        where TVal : IIdentifiable
    {
        // First we check if the object already has a name
        if (!string.IsNullOrEmpty(obj.Name))
            return obj;

        return SetOrGenerateName<TVal, TKey>(obj, name, def);
    }

    private TVal SetOrGenerateName<TVal, TKey>(TVal obj, string? name, Definition? def) where TVal : IIdentifiable
    {
        // Then we check if the name is provided via a definition
        if (def != null)
        {
            string? nameFromDefinition = TryGetNameFromDefinition(def);
            if (nameFromDefinition != null)
            {
                obj.Name = nameFromDefinition;
                return obj;
            }
        }

        // Finally we check if the name is provided as a parameter
        if (!string.IsNullOrEmpty(name))
        {
            obj.Name = name;
            return obj;
        }

        // If no name is provided, we generate one
        Type keyType = typeof(TKey);
        if (!_countersByType.TryGetValue(keyType, out int counter))
        {
            counter = 1;
            _countersByType[keyType] = counter;
        }
        else
        {
            _countersByType[keyType] = counter++;
        }

        obj.Name = $"{keyType.Name}{counter}";

        return obj;
    }

    private static string? TryGetNameFromDefinition(Definition def)
    {
        if (def is NamedDefinition named)
        {
            if (string.IsNullOrEmpty(named.Name.Value))
                throw new Exception("Definition name is empty.");

            return named.Name.Value;
        }

        return null;
    }
}
