
using ScenarioModel.CodeHooks.HookDefinitions;
using ScenarioModel.Collections;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;

namespace ScenarioModel.CodeHooks;

public class DefinitionScope
{
    public List<INodeHookDefinition> NodeHookDefinitions { get; set; } = new();
    public SemiLinearSubGraph<IScenarioNode> SubGraph { get; set; } = null!;

    public void AddNodeDefintion(INodeHookDefinition nodeDefinition)
    {
        NodeHookDefinitions.Add(nodeDefinition);

        SubGraph.NodeSequence.Add(nodeDefinition.GetNode());
    }
}