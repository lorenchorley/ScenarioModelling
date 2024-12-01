using ScenarioModel.Collections;
using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.ContextValidation.ScenarioValidation;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.Visitors;

namespace ScenarioModel.ContextValidation;

public class ScenarioValidator : IScenarioVisitor
{
    private readonly ChooseNodeValidator _chooseNodeValidator = new();
    private readonly DialogNodeValidator _dialogNodeValidator = new();
    private readonly IfNodeValidator _ifNodeValidator = new();
    private readonly JumpNodeValidator _jumpNodeValidator = new();
    private readonly TransitionNodeValidator _transitionNodeValidator = new();
    private readonly WhileNodeValidator _whileNodeValidator = new();

    private System? _system;
    private Scenario? _scenario;

    public ValidationErrors Validate(System system, Scenario scenario)
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
        ArgumentNullException.ThrowIfNull(_system);
        ArgumentNullException.ThrowIfNull(_scenario);

        return _chooseNodeValidator.Validate(_system, _scenario, chooseNode);
    }

    public object VisitDialogNode(DialogNode dialogNode)
    {
        ArgumentNullException.ThrowIfNull(_system);
        ArgumentNullException.ThrowIfNull(_scenario);

        return _dialogNodeValidator.Validate(_system, _scenario, dialogNode);
    }

    public object VisitIfNode(IfNode ifNode)
    {
        ArgumentNullException.ThrowIfNull(_system);
        ArgumentNullException.ThrowIfNull(_scenario);

        return VisitSubgraph(ifNode.SubGraph);
    }

    public object VisitJumpNode(JumpNode jumpNode)
    {
        ArgumentNullException.ThrowIfNull(_system);
        ArgumentNullException.ThrowIfNull(_scenario);

        return _jumpNodeValidator.Validate(_system, _scenario, jumpNode);
    }

    public object VisitTransitionNode(TransitionNode transitionNode)
    {
        ArgumentNullException.ThrowIfNull(_system);
        ArgumentNullException.ThrowIfNull(_scenario);

        return _transitionNodeValidator.Validate(_system, _scenario, transitionNode);
    }

    public object VisitWhileNode(WhileNode whileNode)
    {
        ArgumentNullException.ThrowIfNull(_system);
        ArgumentNullException.ThrowIfNull(_scenario);

        return _whileNodeValidator.Validate(_system, _scenario, whileNode);
    }
}
