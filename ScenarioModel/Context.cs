using ScenarioModel.Serialisation;
using ScenarioModel.Validation;
using System.Text;

namespace ScenarioModel;

public class Context
{
    public List<ISerialiser> Serialisers { get; set; } = new();
    public List<Scenario> Scenarios { get; set; } = new();
    public List<System> Systems { get; set; } = new();
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

    public Context LoadSystem(System system)
    {
        return LoadSystem(system, out _);
    }

    public Context LoadSystem(System newSystem, out System system)
    {
        Systems.Add(newSystem);
        system = newSystem;

        return this;
    }
    
    public Context LoadSystem(string serialisedSystem, out System system)
    {
        foreach (var serialiser in Serialisers)
        {
            var result = serialiser.DeserialiseSystem(serialisedSystem, this);
            if (result.IsSome)
            {
                system = (System)result.Case;
                Systems.Add(system);

                return this;
            }
        }

        throw new Exception("Failed to load system, no serialiser was able to deserialise the incoming text");
    }

    public Context LoadSystem(string serialisedSystem)
    {
        return LoadSystem(serialisedSystem, out _);
    }

    public Context LoadScenario(Scenario scenario)
    {
        return LoadScenario(scenario, out _);
    }

    public Context LoadScenario(Scenario newScenario, out Scenario scenario)
    {
        Scenarios.Add(newScenario);
        scenario = newScenario;
        return this;
    }

    public Context LoadScenario(string serialisedScenario)
    {
        return LoadScenario(serialisedScenario, out _);
    }

    public Context LoadScenario(string serialisedScenario, out Scenario scenario)
    {
        foreach (var serialiser in Serialisers)
        {
            var result = serialiser.DeserialiseScenario(serialisedScenario, this);
            if (result.IsSome)
            {
                scenario = (Scenario)result.Case;
                Scenarios.Add(scenario);

                return this;
            }
        }

        throw new Exception("Failed to load scenario, no serialiser was able to deserialise the incoming text");
    }

    public Context Initialise()
    {
        foreach (var scenario in Scenarios)
        {
            scenario.Initialise(this);
        }
        return this;
    }

    public void Validate()
    {
        foreach (var scenario in Scenarios)
        {
            ValidationErrors.Incorporate(new ScenarioValidator().Validate(scenario));
        }
    }

    public string Serialise<T>() where T : ISerialiser
    {
        StringBuilder sb = new();

        var serialiser = Serialisers.FirstOrDefault(s => s.GetType() == typeof(T));
        if (serialiser == null)
        {
            throw new Exception("Serialiser not found : " + typeof(T).Name);
        }

        foreach (var system in Systems)
        {
            sb.AppendLine(serialiser.SerialiseSystem(system));
            sb.AppendLine();
        }

        foreach (var scenario in Scenarios)
        {
            sb.AppendLine(serialiser.SerialiseScenario(scenario));
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
