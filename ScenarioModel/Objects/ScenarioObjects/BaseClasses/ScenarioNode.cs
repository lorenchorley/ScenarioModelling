using OneOf;
using ScenarioModel.Collections;
using ScenarioModel.Execution.Events;
using ScenarioModel.Objects.ScenarioObjects.DataClasses;
using ScenarioModel.Objects.SystemObjects;

namespace ScenarioModel.Objects.ScenarioObjects.BaseClasses;

public interface IScenarioNode : IDirectedGraphNode<IScenarioNode>, INameful
{
    int? Line { get; set; }
    IScenarioEvent GenerateUntypedEvent(EventGenerationDependencies dependencies);
    OneOfIScenaroNode ToOneOf();
}

public class OneOfIScenaroNode : OneOfBase<ChooseNode, DialogNode, IfNode, JumpNode, StateTransitionNode, WhileNode>
{
    public OneOfIScenaroNode(OneOf<ChooseNode, DialogNode, IfNode, JumpNode, StateTransitionNode, WhileNode> input) : base(input)
    {
    }
}

public abstract record ScenarioNode<E> : IScenarioNode where E : IScenarioEvent
{
    public string Name { get; set; } = "";
    public int? Line { get; set; }
    public abstract E GenerateEvent(EventGenerationDependencies dependencies);
    public abstract IEnumerable<SemiLinearSubGraph<IScenarioNode>> TargetSubgraphs();

    public IScenarioEvent GenerateUntypedEvent(EventGenerationDependencies dependencies)
    {
        return GenerateEvent(dependencies);
    }

    public string? LineInformation
    {
        get => Line.HasValue ? $" (Near line {Line.Value})" : "";
    }

    public abstract OneOfIScenaroNode ToOneOf();
}
