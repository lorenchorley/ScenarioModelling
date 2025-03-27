using ScenarioModelling.CoreObjects.MetaStoryNodes;

namespace ScenarioModelling.CoreObjects.Visitors;

public interface IMetaStoryVisitor
{
    object VisitAssert(AssertNode assertNode);
    object VisitCallMetaStory(CallMetaStoryNode callMetaStory);
    object VisitChoose(ChooseNode chooseNode);
    object VisitDialog(DialogNode dialogNode);
    object VisitIf(IfNode ifNode);
    object VisitJump(JumpNode jumpNode);
    object VisitLoopNode(LoopNode loopNode);
    object VisitMetadata(MetadataNode metadataNode);
    object VisitTransition(TransitionNode transitionNode);
    object VisitWhile(WhileNode whileNode);
}
