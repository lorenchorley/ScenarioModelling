using ScenarioModelling.Collections.Graph;
using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.ContextValidation.ScenarioValidation;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;
using ScenarioModelling.Objects.Visitors;

namespace ScenarioModelling.ContextValidation;

public class ScenarioValidator : IScenarioVisitor
{
    private readonly ChooseNodeValidator _chooseNodeValidator = new();
    private readonly DialogNodeValidator _dialogNodeValidator = new();
    private readonly IfNodeValidator _ifNodeValidator = new();
    private readonly JumpNodeValidator _jumpNodeValidator = new();
    private readonly TransitionNodeValidator _transitionNodeValidator = new();
    private readonly WhileNodeValidator _whileNodeValidator = new();

    private System? _system;
    private MetaStory? _scenario;

    public ValidationErrors Validate(System system, MetaStory scenario)
    {
        SystemObjectExhaustivity.AssertInterfaceExhaustivelyImplemented<IObjectValidator>();

        _system = system;
        _scenario = scenario;

        ValidationErrors validationErrors = new();

        // Name is unique among scenarios
        // InitialNode is in the graph

        validationErrors.Incorporate(VisitSubgraph(scenario.Graph.PrimarySubGraph));

        _system = null;
        _scenario = null;

        return validationErrors;
    }

    private ValidationErrors VisitSubgraph(SemiLinearSubGraph<IScenarioNode> subgraph)
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
        ArgumentNullExceptionStandard.ThrowIfNull(_scenario);

        return _chooseNodeValidator.Validate(_system, _scenario, chooseNode);
    }

    public object VisitDialogNode(DialogNode dialogNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_scenario);

        return _dialogNodeValidator.Validate(_system, _scenario, dialogNode);
    }

    public object VisitIfNode(IfNode ifNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_scenario);

        return VisitSubgraph(ifNode.SubGraph);
    }

    public object VisitJumpNode(JumpNode jumpNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_scenario);

        return _jumpNodeValidator.Validate(_system, _scenario, jumpNode);
    }

    public object VisitTransitionNode(TransitionNode transitionNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_scenario);

        return _transitionNodeValidator.Validate(_system, _scenario, transitionNode);
    }

    public object VisitWhileNode(WhileNode whileNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_system);
        ArgumentNullExceptionStandard.ThrowIfNull(_scenario);

        return _whileNodeValidator.Validate(_system, _scenario, whileNode);
    }
}
