using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using System.Text;

namespace ScenarioModel.Serialisation.HumanReadable.Reserialisation.NodeSerialisers.Interfaces;

public interface INodeSerialiser
{

}

public interface INodeSerialiser<TNode> : INodeSerialiser where TNode : IScenarioNode
{
    void WriteNode(StringBuilder sb, MetaStory scenario, TNode node, string currentIndent);
}

