using LanguageExt.Common;

namespace ScenarioModelling.CoreObjects;

public interface IContextSerialiser
{
    void SetConfigurationOptions(Dictionary<string, string> configuration);
    Result<string> SerialiseContext(Context context);
    Result<Context> DeserialiseContext(string text);
    Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context);
}