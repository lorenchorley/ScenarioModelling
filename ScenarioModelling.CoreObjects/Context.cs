using LanguageExt.Common;
using ScenarioModelling.CoreObjects.ContextValidation;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.MetaStateObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.CoreObjects.References;
using ScenarioModelling.CoreObjects.References.Interfaces;
using ScenarioModelling.CoreObjects.TestCaseNodes;
using ScenarioModelling.Tools.Collections.Graph;
using YamlDotNet.Serialization;
using Relation = ScenarioModelling.CoreObjects.MetaStateObjects.Relation;

namespace ScenarioModelling.CoreObjects;

public class Context
{
    private readonly IServiceProvider _serviceProvider;

    public List<MetaStory> MetaStories { get; private set; } = new();

    public MetaState MetaState { get; private set; }

    public List<TestCase> TestCases { get; private set; } = new();

    [YamlIgnore]
    public List<IContextSerialiser> Serialisers { get; set; } = new();

    [YamlIgnore]
    public ValidationErrors ValidationErrors { get; set; } = new();

    public Context(IServiceProvider serviceProvider, MetaState metaState)
    {
        _serviceProvider = serviceProvider;
        MetaState = metaState;
    }

    public Context UseSerialiser<T>() where T : IContextSerialiser
    {
        T? serialiser = _serviceProvider.GetRequiredService<T>();
        
        Serialisers.Add(serialiser);
        
        return this;
    }
    
    public Context UseSerialiser<T>(Dictionary<string, string> configuration) where T : IContextSerialiser
    {
        T? serialiser = _serviceProvider.GetRequiredService<T>();
        serialiser.SetConfigurationOptions(configuration);

        Serialisers.Add(serialiser);
        
        return this;
    }

    public Context RemoveSerialiser<T>() where T : IContextSerialiser
    {
        T? serialiser = (T?)Serialisers.FirstOrDefault(s => s is T);

        if (serialiser == null)
            throw new Exception($"Serialiser of type {typeof(T).Name} was not registered in context");

        Serialisers.Remove(serialiser);

        return this;
    }

    public Context LoadContext<T>(string serialisedContext) where T : IContextSerialiser
        => LoadContext(typeof(T), serialisedContext);

    public Context LoadContext(string serialisedContext)
        => LoadContext(Serialisers.Single().GetType(), serialisedContext);

    private Context LoadContext(Type serialiserType, string serialisedContext)
    {
        var serialiser = Serialisers.FirstOrDefault(s => s.GetType() == serialiserType);
        if (serialiser == null)
        {
            throw new Exception("Serialiser not found : " + serialiserType.Name);
        }

        var result = serialiser.DeserialiseContext(serialisedContext);

        result.IfFail(e => ValidationErrors.Add(new ContextLoadError(e.Message)));

        return this;
    }

    public Context Initialise()
    {
        if (Serialisers.Count == 0)
        {
            //UseSerialiser<ContextSerialiser>();
            throw new Exception("No serialiser set");
        }

        // Context validation
        ContextValidator? contextValidator = _serviceProvider.GetService<ContextValidator>();
        ValidationErrors.Incorporate(contextValidator.Validate(this));

        return this;
    }

    public Result<string> Serialise<T>() where T : IContextSerialiser
        => Serialise(typeof(T));

    public Result<string> Serialise()
        => Serialise(Serialisers.Single().GetType());

    private Result<string> Serialise(Type serialiserType)
    {
        var serialiser = Serialisers.FirstOrDefault(s => s.GetType() == serialiserType);
        if (serialiser == null)
        {
            throw new Exception("Serialiser not found : " + serialiserType.Name);
        }

        return serialiser.SerialiseContext(this);
    }

    public void CreateObjectsFromUnresolvableReferences()
    {
        var allIdentifiable =
            Enumerable.Empty<IReference>()
                      .Concat(MetaState.AllEntityReferences)
                      .Concat(MetaState.AllEntityTypeReferences)
                      .Concat(MetaState.AllStateReferences)
                      .Concat(MetaState.AllStateMachineReferences)
                      .Concat(MetaState.AllTransitionReferences)
                      .Concat(MetaState.AllAspectReferences)
                      .Concat(MetaState.AllRelationReferences)
                      .Concat(MetaState.AllConstraintReferences);

        foreach (var reference in allIdentifiable.Where(r => !r.IsResolvable()))
        {
            switch (reference) // TODO Exhaustivity ?
            {
                case AspectReference r:
                    MetaState.Aspects.Add(new Aspect(MetaState) 
                    { 
                        Name = r.Name 
                    });

                    break;
                case ConstraintReference r:
                    MetaState.Constraints.Add(new Constraint(MetaState) 
                    { 
                        Name = r.Name 
                    });

                    break;
                case EntityReference r:
                    MetaState.Entities.Add(new Entity(MetaState) 
                    { 
                        Name = r.Name 
                    });

                    break;
                case EntityTypeReference r:
                    MetaState.EntityTypes.Add(new EntityType(MetaState) 
                    { 
                        Name = r.Name, 
                        ExistanceOriginallyInferred = true 
                    });

                    break;
                case RelationReference r:
                    MetaState.Relations.Add(new Relation(MetaState) 
                    { 
                        Name = r.Name 
                    });

                    break;
                case StateReference r:
                    MetaState.States.Add(new State(MetaState) 
                    { 
                        Name = r.Name 
                    });

                    break;
                case StateMachineReference r:
                    MetaState.StateMachines.Add(new StateMachine(MetaState) 
                    { 
                        Name = r.Name, 
                        ExistanceOriginallyInferred = true 
                    });

                    break;
                case TransitionReference r:
                    MetaState.Transitions.Add(new Transition(MetaState) 
                    { 
                        Name = r.Name 
                    });

                    break;
                default:
                    throw new NotImplementedException($"Reference type {reference.GetType().Name} not implemented.");
            }
        }
    }

    public Context ResetToInitialState()
    {
        foreach (var statefulObject in MetaState.AllStateful)
        {
            if (!statefulObject.State.IsSet)
            {
                // If no state is set, that is that no state machine is associated with the stateful object
                // Then we should not try to reset the state to it's initial state
                continue;
            }

            State? resolvedValue = statefulObject.InitialState.ResolvedValue;

            if (resolvedValue == null)
            {
                throw new Exception($"Stateful object {statefulObject.Name} has no initial state");
            }

            statefulObject.State.SetReference(resolvedValue.GenerateReference());
        }

        return this;
    }

    internal MetaStory NewMetaStory(string name, ISubGraph<IStoryNode> primarySubGraph)
    {
        MetaStory metaStory = new(MetaState, new(primarySubGraph)) { Name = name };
        MetaStories.Add(metaStory);
        return metaStory;
    }

}
