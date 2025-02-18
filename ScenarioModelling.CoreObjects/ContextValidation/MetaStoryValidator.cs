using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;
using ScenarioModelling.CoreObjects.StoryNodes;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.CoreObjects.Visitors;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CoreObjects.ContextValidation;

public class MetaStoryValidator : IMetaStoryVisitor
{
    private readonly CallMetaStoryNodeValidator _callMetaStoryNodeValidator = new();
    private readonly ChooseNodeValidator _chooseNodeValidator = new();
    private readonly DialogNodeValidator _dialogNodeValidator = new();
    private readonly IfNodeValidator _ifNodeValidator = new();
    private readonly JumpNodeValidator _jumpNodeValidator = new();
    private readonly MetadataNodeValidator _metadataNodeValidator = new();
    private readonly TransitionNodeValidator _transitionNodeValidator = new();
    private readonly WhileNodeValidator _whileNodeValidator = new();

    private MetaState? _metaState;
    private MetaStory? _metaStory;

    public ValidationErrors Validate(MetaState metaState, MetaStory MetaStory)
    {
        _metaState = metaState;
        _metaStory = MetaStory;

        ValidationErrors validationErrors = new();

        // Name is unique among MetaStories
        // InitialNode is in the graph

        validationErrors.Incorporate(VisitSubgraph(MetaStory.Graph.PrimarySubGraph));

        _metaState = null;
        _metaStory = null;

        return validationErrors;
    }

    private ValidationErrors VisitSubgraph(SemiLinearSubGraph<IStoryNode> subgraph)
    {
        ValidationErrors validationErrors = new();

        foreach (var node in subgraph.NodeSequence)
        {
            validationErrors.Incorporate((ValidationErrors)node.Accept(this));
        }

        return validationErrors;
    }

    public object VisitCallMetaStory(CallMetaStoryNode callMetaStoryNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaState);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _callMetaStoryNodeValidator.Validate(_metaState, _metaStory, callMetaStoryNode);
    }

    public object VisitChooseNode(ChooseNode chooseNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaState);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _chooseNodeValidator.Validate(_metaState, _metaStory, chooseNode);
    }

    public object VisitDialogNode(DialogNode dialogNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaState);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _dialogNodeValidator.Validate(_metaState, _metaStory, dialogNode);
    }

    public object VisitIfNode(IfNode ifNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaState);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        var errors = _ifNodeValidator.Validate(_metaState, _metaStory, ifNode);

        errors.Incorporate(VisitSubgraph(ifNode.SubGraph));

        return errors;
    }

    public object VisitJumpNode(JumpNode jumpNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaState);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _jumpNodeValidator.Validate(_metaState, _metaStory, jumpNode);
    }

    public object VisitMetadataNode(MetadataNode metadataNode)
    {
        return _metadataNodeValidator.Validate(_metaState, _metaStory, metadataNode);
    }

    public object VisitTransitionNode(TransitionNode transitionNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaState);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _transitionNodeValidator.Validate(_metaState, _metaStory, transitionNode);
    }

    public object VisitWhileNode(WhileNode whileNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_metaState);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        var errors = _whileNodeValidator.Validate(_metaState, _metaStory, whileNode);

        errors.Incorporate(VisitSubgraph(whileNode.SubGraph));

        return errors;
    }

}
