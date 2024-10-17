using LanguageExt.Common;
using ScenarioModel.Serialisation;
using ScenarioModel.Validation;

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
    {
        var serialiser = Serialisers.FirstOrDefault(s => s.GetType() == typeof(T));
        if (serialiser == null)
        {
            throw new Exception("Serialiser not found : " + typeof(T).Name);
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
        foreach (var scenario in Scenarios)
        {
            scenario.Initialise(this);
        }

        System.Initialise();

        Validate();

        return this;
    }

    public void Validate()
    {
        foreach (var scenario in Scenarios)
        {
            ValidationErrors.Incorporate(new ScenarioValidator().Validate(scenario));
        }

        ValidationErrors.Incorporate(new SystemValidator().Validate(System));
    }

    public Result<string> Serialise<T>() where T : ISerialiser
    {
        var serialiser = Serialisers.FirstOrDefault(s => s.GetType() == typeof(T));
        if (serialiser == null)
        {
            throw new Exception("Serialiser not found : " + typeof(T).Name);
        }

        return serialiser.SerialiseContext(this);
    }

    public Context Incorporate(Context newContext)
    {
        Scenarios.AddRange(newContext.Scenarios);

        System.Entities.AddRange(newContext.System.Entities);
        System.EntityTypes.AddRange(newContext.System.EntityTypes);
        System.StateMachines.AddRange(newContext.System.StateMachines);
        System.Constraints.AddRange(newContext.System.Constraints);

        return this;
    }
}
