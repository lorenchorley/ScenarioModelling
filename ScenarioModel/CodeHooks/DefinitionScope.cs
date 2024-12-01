using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.CodeHooks.HookDefinitions.ScenarioObjects;
using ScenarioModel.Collections;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks;

public class DefinitionScope
{
    public List<INodeHookDefinition> NodeHookDefinitions { get; set; } = new();
    public SemiLinearSubGraph<IScenarioNode> SubGraph { get; set; } = null!;
    public int CurrentIndex { get; set; } = 0;

    public void AddNodeDefintion(INodeHookDefinition nodeDefinition)
    {
        NodeHookDefinitions.Add(nodeDefinition);

        SubGraph.NodeSequence.Add(nodeDefinition.GetNode());
        CurrentIndex++;
    }

    internal void SetCurrentNodeDefintion(WhileHookDefinition whileHookDefinition)
    {
        var node = whileHookDefinition.GetNode();
        int? index = SubGraph.NodeSequence.IndexOf(node);

        if (index is null)
        {
            throw new Exception("Node not found in subgraph");
        }

        CurrentIndex = (int)index;
    }
}