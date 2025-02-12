using LanguageExt.Common;

namespace ScenarioModelling.CoreObjects;

public interface IContextSerialiser
{
    Result<string> SerialiseContext(Context context);
    Result<Context> DeserialiseContext(string text);
    Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context);
}