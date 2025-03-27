using LanguageExt;
using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Serialisation.Expressions.Interpreter;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;

[StoryNodeLike<IDefinitionToNodeDeserialiser, LoopNode>]
public class LoopNodeDeserialiser : IDefinitionToNodeDeserialiser
{
    [StoryNodeLikeProperty]
    public string Name => "Loop".ToUpperInvariant();

    [StoryNodeLikeProperty]
    public Func<Definition, bool>? Predicate => null;

    public IStoryNode Transform(Definition def, MetaStory metaStory, SemiLinearSubGraph<IStoryNode> currentSubgraph, Func<Definition, SemiLinearSubGraph<IStoryNode>, Option<IStoryNode>> tryTransform)
    {
        if (def is not UnnamedDefinition expDef)
        {
            throw new Exception("Loop node must be expression definition");
        }

        LoopNode node = new();
        node.Line = def.Line;

        def.HasBeenTransformed = true;

        //node.SubGraph.ParentSubgraph = currentSubgraph;
        //node.SubGraph.ReentryPoint = node; // Node is probaby wrong here
        node.SubGraph.AddRangeToSequence(expDef.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, node.SubGraph), "Unknown node types not taken into account : {0}").ToList());

        return node;
    }
}

