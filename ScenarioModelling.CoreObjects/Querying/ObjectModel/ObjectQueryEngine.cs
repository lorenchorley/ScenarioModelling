using ScenarioModelling.CoreObjects;
using ScenarioModelling.Querying.ObjectModel.Defunctionalisation;

namespace ScenarioModelling.Querying.ObjectModel;

public class ObjectQueryEngine
{
    public ICoreObject ExecuteQuery(IObjectQueryNode rootNode, Context context)
    {
        throw new NotImplementedException("ExecuteQuery");

    }

    public string SerialiseToYaml(IObjectQueryNode rootNode)
    {
        throw new NotImplementedException("Serialisation from YAML is not implemented yet.");
    }

    public static IObjectQueryNode DeserialiseFromYaml(string yaml)
    {
        throw new NotImplementedException("Deserialisation from YAML is not implemented yet.");
    }
}
