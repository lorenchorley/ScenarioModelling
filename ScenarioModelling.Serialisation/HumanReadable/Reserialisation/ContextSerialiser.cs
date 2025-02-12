using LanguageExt.Common;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.Interpreter;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation.WorkInProgress;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation;

public class ContextSerialiser : IContextSerialiser
{
    internal const string IndentSegment = "  ";

    private readonly MetaStorySerialiser _metaStorySerialiser;
    private readonly MetaStateSerialiser _systemSerialiser;
    private readonly ContextDeserialiser _contextBuilder;

    public ContextSerialiser(ContextDeserialiser contextBuilder, MetaStorySerialiser metaStorySerialiser, MetaStateSerialiser systemSerialiser)
    {
        _contextBuilder = contextBuilder;
        _metaStorySerialiser = metaStorySerialiser;
        _systemSerialiser = systemSerialiser;
    }

    public Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context)
    {
        var newContext = DeserialiseContext(text);

        return newContext.Match(Succ: c => c, Fail: e => new Result<Context>(e));
    }

    public Result<Context> DeserialiseContext(string text)
    {
        HumanReadableInterpreter interpreter = new();
        var result = interpreter.Parse(text);

        if (result.HasErrors)
        {
            return new Result<Context>(new Exception(string.Join("\n", result.Errors)));
        }

        ContextBuilderInputs inputs = new();
        inputs.DefinitionTreeTopLevel.AddRange(result.ParsedObject!);

        return _contextBuilder.RefreshContextWithInputs(inputs);
    }

    public Result<string> SerialiseContext(Context context)
    {
        StringBuilder sb = new();
        StrMkr sm = new();

        _systemSerialiser.WriteSystem(sb, context.MetaState, "");

        foreach (var metaStory in context.MetaStories)
        {
            _metaStorySerialiser.WriteMetaStory(sb, metaStory, "");
        }

        return sb.ToString();
    }

}
