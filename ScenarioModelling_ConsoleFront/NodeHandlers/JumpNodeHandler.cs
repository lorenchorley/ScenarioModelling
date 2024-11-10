using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

[NodeLike<INodeHandler, JumpNode>]
internal class JumpNodeHandler : NodeHandler<JumpNode, JumpEvent>
{
    public override void Handle(JumpNode node, JumpEvent e)
    {
    }
}
