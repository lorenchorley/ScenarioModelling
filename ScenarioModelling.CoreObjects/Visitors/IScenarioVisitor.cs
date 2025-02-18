using ScenarioModelling.CoreObjects.StoryNodes;

namespace ScenarioModelling.CoreObjects.Visitors;

public interface IMetaStoryVisitor
{
    object VisitCallMetaStory(CallMetaStoryNode callMetaStory);
    object VisitChooseNode(ChooseNode chooseNode);
    object VisitDialogNode(DialogNode dialogNode);
    object VisitIfNode(IfNode ifNode);
    object VisitJumpNode(JumpNode jumpNode);
    object VisitMetadataNode(MetadataNode metadataNode);
    object VisitTransitionNode(TransitionNode transitionNode);
    object VisitWhileNode(WhileNode whileNode);
}
