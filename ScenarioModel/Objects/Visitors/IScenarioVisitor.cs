using ScenarioModelling.Objects.StoryNodes;

namespace ScenarioModelling.Objects.Visitors;

public interface IMetaStoryVisitor
{
    object VisitChooseNode(ChooseNode chooseNode);
    object VisitDialogNode(DialogNode dialogNode);
    object VisitIfNode(IfNode ifNode);
    object VisitJumpNode(JumpNode jumpNode);
    object VisitMetadataNode(MetadataNode metadataNode);
    object VisitTransitionNode(TransitionNode transitionNode);
    object VisitWhileNode(WhileNode whileNode);
}
