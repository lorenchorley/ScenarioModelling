using LanguageExt.Common;
using ScenarioModel.ContextValidation;
using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;
using ScenarioModel.Serialisation;
using Relation = ScenarioModel.Objects.SystemObjects.Relation;

namespace ScenarioModel;

public class Context
{
    public List<ISerialiser> Serialisers { get; set; } = new();
    public List<Scenario> Scenarios { get; set; } = new();
    public System System { get; set; } = new();
    public ValidationErrors ValidationErrors { get; set; } = new();

    private Context()
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

        foreach (var scenario in Scenarios)
        {
            scenario.System = system;
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

    //            foreach (var scenario in Scenarios)
    //            {
    //                scenario.System = system;
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

    public Context LoadScenario(Scenario scenario)
    {
        return LoadScenario(scenario, out _);
    }

    public Context LoadScenario(Scenario newScenario, out Scenario scenario)
    {
        Scenarios.Add(newScenario);
        newScenario.System = System;
        scenario = newScenario;
        return this;
    }

    //public Context LoadScenarios(string serialisedScenarios)
    //{
    //    return LoadScenarios(serialisedScenarios, out _);
    //}

    //public Context LoadScenarios(string serialisedScenarios, out List<Scenario> scenarios)
    //{
    //    foreach (var serialiser in Serialisers)
    //    {
    //        var result = serialiser.DeserialiseScenarios(serialisedScenarios, this);
    //        if (result.IsSome)
    //        {
    //            scenarios = (List<Scenario>)result.Case;

    //            foreach (Scenario scenario in scenarios)
    //            {
    //                scenario.System = System;
    //                Scenarios.Add(scenario);
    //            }

    //            return this;
    //        }
    //    }

    //    throw new Exception("Failed to load scenario, no serialiser was able to deserialise the incoming text");
    //}

    public Context Initialise()
    {
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
        Scenarios.AddRange(newContext.Scenarios);

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

}
