using LanguageExt.Common;
using ProtoBuf;
using ScenarioModelling.ContextValidation;
using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Objects.SystemObjects;
using ScenarioModelling.References;
using ScenarioModelling.References.Interfaces;
using ScenarioModelling.Serialisation;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using YamlDotNet.Serialization;
using Relation = ScenarioModelling.Objects.SystemObjects.Relation;

namespace ScenarioModelling;

[ProtoContract]
public class Context
{
    [ProtoMember(1)]
    public List<MetaStory> MetaStories { get; set; } = new();
    [ProtoMember(2)]
    public System System { get; set; } = new();

    [YamlIgnore]
    public List<ISerialiser> Serialisers { get; set; } = new();
    [YamlIgnore]
    public ValidationErrors ValidationErrors { get; set; } = new();

    public Context()
    {
    }

    public static Context New()
    {
        return new Context();
    }

    public Context UseSerialiser<T>() where T : ISerialiser, new()
    {
        Serialisers.Add(new T());
        return this;
    }

    public Context LoadContext<T>(string serialisedContext) where T : ISerialiser
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

        var result = serialiser.DeserialiseExtraContextIntoExisting(serialisedContext, this);

        result.IfFail(e => ValidationErrors.Add(new ContextLoadError(e.Message)));

        return this;
    }

    public Context LoadSystem(System system)
    {
        return LoadSystem(system, out _);
    }

    public Context LoadSystem(System newSystem, out System system)
    {
        System = newSystem;
        system = newSystem;

        foreach (var MetaStory in MetaStories)
        {
            MetaStory.System = system;
        }

        return this;
    }

    //public Context LoadSystem(string serialisedSystem, out System system)
    //{
    //    foreach (var serialiser in Serialisers)
    //    {
    //        var result = serialiser.DeserialiseSystem(serialisedSystem, this);
    //        if (result.IsSome)
    //        {
    //            system = (System)result.Case;
    //            System = system;

    //            foreach (var MetaStory in MetaStories)
    //            {
    //                MetaStory.System = system;
    //            }

    //            return this;
    //        }
    //    }

    //    throw new Exception("Failed to load system, no serialiser was able to deserialise the incoming text");
    //}

    //public Context LoadSystem(string serialisedSystem)
    //{
    //    return LoadSystem(serialisedSystem, out _);
    //}

    public Context LoadMetaStory(MetaStory MetaStory)
    {
        return LoadMetaStory(MetaStory, out _);
    }

    public Context LoadMetaStory(MetaStory newMetaStory, out MetaStory MetaStory)
    {
        MetaStories.Add(newMetaStory);
        newMetaStory.System = System;
        MetaStory = newMetaStory;
        return this;
    }

    //public Context LoadMetaStories(string serialisedMetaStories)
    //{
    //    return LoadMetaStories(serialisedMetaStories, out _);
    //}

    //public Context LoadMetaStories(string serialisedMetaStories, out List<MetaStory> MetaStories)
    //{
    //    foreach (var serialiser in Serialisers)
    //    {
    //        var result = serialiser.DeserialiseMetaStories(serialisedMetaStories, this);
    //        if (result.IsSome)
    //        {
    //            MetaStories = (List<MetaStory>)result.Case;

    //            foreach (MetaStory MetaStory in MetaStories)
    //            {
    //                MetaStory.System = System;
    //                MetaStories.Add(MetaStory);
    //            }

    //            return this;
    //        }
    //    }

    //    throw new Exception("Failed to load MetaStory, no serialiser was able to deserialise the incoming text");
    //}

    public Context Initialise()
    {
        if (Serialisers.Count == 0)
        {
            UseSerialiser<ContextSerialiser>();
        }

        // Context validation
        ValidationErrors.Incorporate(new ContextValidator().Validate(this));

        return this;
    }

    public Result<string> Serialise<T>() where T : ISerialiser
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

    public Context Incorporate(Context newContext)
    {
        MetaStories.AddRange(newContext.MetaStories); // TODO Merge existing meta stories with the same name, or throw on inconsistency

        SystemObjectExhaustivity.DoForEachObjectType(
            entity: () => System.Entities.AddRange(newContext.System.Entities),
            entityType: () => System.EntityTypes.AddRange(newContext.System.EntityTypes),
            aspect: () => System.Aspects.AddRange(newContext.System.Aspects),
            relation: () => System.Relations.AddRange(newContext.System.Relations),
            state: () => System.States.AddRange(newContext.System.States),
            stateMachine: () => System.StateMachines.AddRange(newContext.System.StateMachines),
            transition: () => System.Transitions.AddRange(newContext.System.Transitions),
            constraint: () => System.Constraints.AddRange(newContext.System.Constraints)
        );

        return this;
    }

    public Context SetResourceFolder(string v)
    {
        // TODO

        return this;
    }


    public void CreateObjectsFromUnresolvableReferences()
    {
        var allIdentifiable =
            Enumerable.Empty<IReference>()
                      .Concat(System.AllEntityReferences)
                      .Concat(System.AllEntityTypeReferences)
                      .Concat(System.AllStateReferences)
                      .Concat(System.AllStateMachineReferences)
                      .Concat(System.AllTransitionReferences)
                      .Concat(System.AllAspectReferences)
                      .Concat(System.AllRelationReferences)
                      .Concat(System.AllConstraintReferences);

        foreach (var reference in allIdentifiable)
        {
            if (reference.IsResolvable())
                continue;

            switch (reference) // TODO Exhaustivity ?
            {
                case EntityReference r:
                    new Entity(System) { Name = r.Name };
                    break;
                case EntityTypeReference r:
                    new EntityType(System) { Name = r.Name, ExistanceOriginallyInferred = true };
                    break;
                case AspectReference r:
                    new Aspect(System) { Name = r.Name };
                    break;
                //case AspectTypeReference r:
                //    new AspectType(_context.System) { Name = r.Name, ExistanceOriginallyInferred = true };
                //    break;
                case StateReference r:
                    new State(System) { Name = r.Name };
                    break;
                case StateMachineReference r:
                    new StateMachine(System) { Name = r.Name, ExistanceOriginallyInferred = true };
                    break;
                case TransitionReference r:
                    new Transition(System) { Name = r.Name };
                    break;
                case RelationReference r:
                    new Relation(System) { Name = r.Name };
                    break;
                case ConstraintReference r:
                    new Constraint(System) { Name = r.Name };
                    break;
                default:
                    throw new NotImplementedException($"Reference type {reference.GetType().Name} not implemented.");
            }
        }
    }

    public Context ResetToInitialState()
    {
        foreach (var statefulObject in System.AllStateful)
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
}
