using LanguageExt;

namespace ScenarioModel.Serialisation;

public interface ISerialiser
{
    string SerialiseSystem(System system);
    Option<System> DeserialiseSystem(string text, Context context);

    string SerialiseScenario(Scenario scenario);
    Option<Scenario> DeserialiseScenario(string text, Context context);
}