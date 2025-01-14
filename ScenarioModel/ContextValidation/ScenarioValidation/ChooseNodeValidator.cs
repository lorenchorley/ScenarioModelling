using ScenarioModel.Collections.Graph;
using ScenarioModel.ContextValidation.Errors;
using ScenarioModel.ContextValidation.Interfaces;
using ScenarioModel.Exhaustiveness.Attributes;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, ChooseNode>]
public class ChooseNodeValidator : INodeValidator<ChooseNode>
{
    public ValidationErrors Validate(System system, MetaStory scenario, ChooseNode node)
    {
        ValidationErrors validationErrors = new();

        foreach (var choice in node.Choices)
        {
            //validationErrors.AddIfNot(IsNodeName(graph, choice.NodeName), new InvalidNodeName($"Node {choice} not found on {typeof(ChooseNode)} {node.Name}"));
        }

        return validationErrors;
    }

    private bool IsNodeName(SemiLinearSubGraph<IScenarioNode> graph, string name)
    {
        return graph.NodeSequence.Any(s => s.Name.IsEqv(name));
    }
}