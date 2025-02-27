using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStateObjects.Interfaces;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.SystemObjectSerialisers.Interfaces;

public interface IObjectSerialiser
{

}

public interface IObjectSerialiser<TObject> : IObjectSerialiser where TObject : ISystemObject
{
    void WriteObject(StringBuilder sb, MetaState metaState, TObject obj, string currentIndent);
}

