using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioObjects;
using ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

[NodeLike<INodeHandler, IfNode>]
internal class IfNodeHandler : NodeHandler<IfNode, IfBlockEvent>
{
    public override void Handle(IfNode node, IfBlockEvent e)
    {

    }
}
