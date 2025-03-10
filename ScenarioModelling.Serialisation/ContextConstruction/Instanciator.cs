using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.GenericInterfaces;
using Relation = ScenarioModelling.CoreObjects.MetaStateObjects.Relation;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;

namespace ScenarioModelling.Serialisation.ContextConstruction;

public class Instanciator
{
    private Dictionary<Type, int> _countersByType = new();
    private readonly Context _context;

    public Instanciator(Context context)
    {
        _context = context;
    }

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

    public MetaStory NewMetaStory(Definition definition, ISubGraph<IStoryNode> subGraph)
    {
        MetaStory MetaStory = _context.NewMetaStory("", subGraph); // Must not have a name here so that the method Name can do it's job

        return Name<MetaStory, MetaStory>(MetaStory, def: definition);
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
        MetaStateObjectExhaustivity.AssertIsObjectType<TVal>();

        object instance = typeof(TVal).Name switch // TODO Exhaustivity ?
        {
            nameof(Entity) => new Entity(_context.MetaState),
            nameof(EntityType) => new EntityType(_context.MetaState),
            nameof(Aspect) => new Aspect(_context.MetaState),
            nameof(State) => new State(_context.MetaState),
            nameof(StateMachine) => new StateMachine(_context.MetaState),
            nameof(Transition) => new Transition(_context.MetaState),
            nameof(Relation) => new Relation(_context.MetaState),
            nameof(Constraint) => new Constraint(_context.MetaState),
            _ => throw new NotImplementedException($"Object type {typeof(TVal).Name} not implemented.")
        };

        return Name<TVal, TVal>((TVal)instance, name, definition);
    }

    public TRef NewReference<TVal, TRef>(string? name = null, Definition? definition = null)
        where TVal : IIdentifiable
        where TRef : IReference<TVal>
    {
        MetaStateObjectExhaustivity.AssertIsObjectType<TVal>();

        object reference = typeof(TRef).Name switch // TODO Exhaustivity ?
        {
            nameof(EntityReference) => new EntityReference(_context.MetaState),
            nameof(EntityTypeReference) => new EntityTypeReference(_context.MetaState),
            nameof(AspectReference) => new AspectReference(_context.MetaState),
            nameof(StateReference) => new StateReference(_context.MetaState),
            nameof(StateMachineReference) => new StateMachineReference(_context.MetaState),
            nameof(TransitionReference) => throw new Exception("Transitino reference might not have enough information (source, destination)"),
            nameof(RelationReference) => new RelationReference(_context.MetaState),
            nameof(ConstraintReference) => new ConstraintReference(_context.MetaState),
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

        // If no name is provided, we generate one if name generation is required
        if (IsNameGenerationRequired<TKey>())
        {
            obj.Name = GenerateName<TKey>();
        }

        return obj;
    }

    private static bool IsNameGenerationRequired<TKey>()
    {
        return typeof(TKey) != typeof(Transition) &&
               typeof(TKey) != typeof(Relation);
    }

    private string GenerateName<TKey>()
    {
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

        string name1 = $"{keyType.Name}{counter}";
        return name1;
    }

    private static string? TryGetNameFromDefinition(Definition def)
    {
        if (def is NamedDefinition named)
        {
            if (string.IsNullOrEmpty(named.Name.Value))
                throw new Exception("Definition name is empty.");

            return named.Name.Value;
        }

        if (def is NamedLinkDefinition namedLink)
        {
            if (string.IsNullOrEmpty(namedLink.Name.Value))
                throw new Exception("Definition name is empty.");

            return namedLink.Name.Value;
        }

        return null;
    }
}
