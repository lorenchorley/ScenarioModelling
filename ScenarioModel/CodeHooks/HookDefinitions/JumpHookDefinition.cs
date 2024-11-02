using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;

namespace ScenarioModel.CodeHooks.HookDefinitions;

[NodeLike<INodeHookDefinition, JumpNode>]
public class JumpHookDefinition(string Name) : INodeHookDefinition
{

}
