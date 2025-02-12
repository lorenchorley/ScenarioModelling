using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.SystemObjects.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.SystemObjectSerialisers.Interfaces;

public interface IObjectSerialiser
{

}

public interface IObjectSerialiser<TObject> : IObjectSerialiser where TObject : ISystemObject
{
    void WriteObject(StringBuilder sb, MetaState metaState, TObject obj, string currentIndent);
}

