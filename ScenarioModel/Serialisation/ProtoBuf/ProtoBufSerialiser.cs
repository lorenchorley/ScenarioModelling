using LanguageExt.Common;
using ScenarioModelling.Extensions;

namespace ScenarioModelling.Serialisation.ProtoBuf;

public class ProtoBufSerialiser : ISerialiser
{
    public Result<Context> DeserialiseContext(string text)
    {
        return SerialisationFunctions.ProtoBufDecompressAndDeserialize<Context>(text);
    }

    public Result<string> SerialiseContext(Context context)
    {
        return SerialisationFunctions.ProtoBufSerializeAndCompress(context);
    }

    public Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context)
    {
        var newContext = DeserialiseContext(text);

        return newContext.Match(Succ: c => context.Incorporate(c), Fail: e => new Result<Context>(e));
    }
}
