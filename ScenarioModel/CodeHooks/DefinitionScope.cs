
using ScenarioModel.CodeHooks.HookDefinitions;

namespace ScenarioModel.CodeHooks;

public class DefinitionScope
{
    public List<INodeHookDefinition> NodeHookDefinitions { get; set; } = new();

    public void AddNodeDefintion(INodeHookDefinition nodeDefinition)
    {
        NodeHookDefinitions.Add(nodeDefinition);
    }
}