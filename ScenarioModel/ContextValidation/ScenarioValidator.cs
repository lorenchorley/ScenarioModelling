using ScenarioModelling.Collections.Graph;
using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.ContextValidation.MetaStoryValidation;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.Visitors;

namespace ScenarioModelling.ContextValidation;

public class MetaStoryValidator : IMetaStoryVisitor
{
    private readonly ChooseNodeValidator _chooseNodeValidator = new();
    private readonly DialogNodeValidator _dialogNodeValidator = new();
    private readonly IfNodeValidator _ifNodeValidator = new();
    private readonly JumpNodeValidator _jumpNodeValidator = new();
    private readonly TransitionNodeValidator _transitionNodeValidator = new();
    private readonly WhileNodeValidator _whileNodeValidator = new();

    private System? _system;
    private MetaStory? _metaStory;

    public ValidationErrors Validate(System system, MetaStory MetaStory)
    {
        SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IObjectValidator>();

        _system = system;
        _metaStory = MetaStory;

        ValidationErrors validationErrors = new();

        // Name is unique among MetaStories
        // InitialNode is in the graph

        validationErrors.Incorporate(VisitSubgraph(MetaStory.Graph.PrimarySubGraph));

        _system = null;
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

    public object VisitChooseNode(ChooseNode chooseNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _chooseNodeValidator.Validate(_system, _metaStory, chooseNode);
    }

    public object VisitDialogNode(DialogNode dialogNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _dialogNodeValidator.Validate(_system, _metaStory, dialogNode);
    }

    public object VisitIfNode(IfNode ifNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return VisitSubgraph(ifNode.SubGraph);
    }

    public object VisitJumpNode(JumpNode jumpNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _jumpNodeValidator.Validate(_system, _metaStory, jumpNode);
    }

    public object VisitTransitionNode(TransitionNode transitionNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _transitionNodeValidator.Validate(_system, _metaStory, transitionNode);
    }

    public object VisitWhileNode(WhileNode whileNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_metaStory);

        return _whileNodeValidator.Validate(_system, _metaStory, whileNode);
    }
}
