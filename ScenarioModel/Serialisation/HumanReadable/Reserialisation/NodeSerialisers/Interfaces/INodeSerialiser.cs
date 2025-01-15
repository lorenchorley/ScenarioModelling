using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;

public interface INodeSerialiser
{

}

public interface INodeSerialiser<TNode> : INodeSerialiser where TNode : IScenarioNode
{
    void WriteNode(StringBuilder sb, MetaStory scenario, TNode node, string currentIndent);
}

