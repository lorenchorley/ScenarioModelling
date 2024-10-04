using LanguageExt;
using LanguageExt.Common;

namespace ScenarioModel.Serialisation;

public interface ISerialiser
{
    Result<string> SerialiseContext(Context context);
    Result<Context> DeserialiseContext(string text);
    Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context);
}