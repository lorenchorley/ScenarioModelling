using ScenarioModel.Execution.Events;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModelling_ConsoleFront.NodeHandlers.BaseClasses;

namespace ScenarioModelling_ConsoleFront.NodeHandlers;

[NodeLike<INodeHandler, WhileNode>]
internal class WhileNodeHandler : NodeHandler<WhileNode, WhileLoopConditionCheckEvent>
{
    public override void Handle(WhileNode node, WhileLoopConditionCheckEvent e)
    {

    }
}
