using ScenarioModelling.CoreObjects.MetaStoryNodes;

namespace ScenarioModelling.CoreObjects.Visitors;

public interface IMetaStoryAsyncVisitor
{
    Task<object> VisitAssert(AssertNode assertNode);
    Task<object> VisitCallMetaStory(CallMetaStoryNode callMetaStory);
    Task<object> VisitChoose(ChooseNode chooseNode);
    Task<object> VisitDialog(DialogNode dialogNode);
    Task<object> VisitIf(IfNode ifNode);
    Task<object> VisitJump(JumpNode jumpNode);
    Task<object> VisitLoopNode(LoopNode loopNode);
    Task<object> VisitMetadata(MetadataNode metadataNode);
    Task<object> VisitTransition(TransitionNode transitionNode);
    Task<object> VisitWhile(WhileNode whileNode);
}
