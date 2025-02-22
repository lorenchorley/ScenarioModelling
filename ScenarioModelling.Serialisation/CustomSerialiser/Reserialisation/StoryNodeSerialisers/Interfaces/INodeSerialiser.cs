using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation.StoryNodeSerialisers.Interfaces;

public interface INodeSerialiser
{

}

public interface INodeSerialiser<TNode> : INodeSerialiser where TNode : IStoryNode
{
    void WriteNode(StringBuilder sb, MetaStory MetaStory, TNode node, string currentIndent);
}

