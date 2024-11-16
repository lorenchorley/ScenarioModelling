using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

[NodeLike<INodeHandler, IfNode>]
internal class IfNodeHandler : NodeHandler<IfNode, IfBlockEvent>
{
    public override void Handle(IfNode node, IfBlockEvent e)
    {

    }
}
