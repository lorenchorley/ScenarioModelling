using LanguageExt.Common;
using ScenarioModelling.CoreObjects;

namespace ScenarioModelling.Serialisation.Export.PlantUML.SequenceDiagram;

public class PlantUMLSequenceDiagramExporter : IContextSerialiser
{
    public Result<Context> DeserialiseContext(string text)
    {
        throw new NotImplementedException();
    }

    public Result<Context> DeserialiseExtraContextIntoExisting(string text, Context context)
    {
        throw new NotImplementedException();
    }

    public Result<string> SerialiseContext(Context context)
    {
        throw new NotImplementedException();
    }
}