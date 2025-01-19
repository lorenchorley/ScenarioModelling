﻿using LanguageExt.Common;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.ContextConstruction;
using ScenarioModelling.Serialisation.HumanReadable.Deserialisation.Interpreter;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation;

public class ContextSerialiser : ISerialiser
{
    private const string _indentSegment = "  ";

    private readonly MetaStorySerialiser _metaStorySerialiser;
    private readonly SystemSerialiser _systemSerialiser;

    public ContextSerialiser()
    {
        _metaStorySerialiser = new(_indentSegment);
        _systemSerialiser = new(_indentSegment);
    }

    public Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context)
    {
        var newContext = DeserialiseContext(text);

        return newContext.Match(Succ: c => context.Incorporate(c), Fail: e => new Result<Context>(e));
    }

    public Result<Context> DeserialiseContext(string text)
    {
        HumanReadableInterpreter interpreter = new();
        var result = interpreter.Parse(text);

        if (result.HasErrors)
        {
            return new Result<Context>(new Exception(string.Join("\n", result.Errors)));
        }

        ContextDeserialiser contextBuilder = new();

        ContextBuilderInputs inputs = new();
        inputs.DefinitionTreeTopLevel.AddRange(result.ParsedObject!);

        return contextBuilder.RefreshContextWithInputs(inputs);
    }

    public Result<string> SerialiseContext(Context context)
    {
        StringBuilder sb = new();
        StrMkr sm = new();

        _systemSerialiser.WriteSystem(sb, context.System, "");

        foreach (var metaStory in context.MetaStories)
        {
            _metaStorySerialiser.WriteMetaStory(sb, metaStory, "");
        }

        return sb.ToString();
    }

}
