using ScenarioModelling.Collections.Graph;
using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.ContextValidation.ScenarioValidation;

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