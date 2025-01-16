using ScenarioModelling.Collections.Graph;
using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.StoryNodes;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, ChooseNode>]
public class ChooseNodeValidator : INodeValidator<ChooseNode>
{
    public ValidationErrors Validate(System system, MetaStory MetaStory, ChooseNode node)
    {
        ValidationErrors validationErrors = new();

        foreach (var choice in node.Choices)
        {
            //validationErrors.AddIfNot(IsNodeName(graph, choice.NodeName), new InvalidNodeName($"Node {choice} not found on {typeof(ChooseNode)} {node.Name}"));
        }

        return validationErrors;
    }

    private bool IsNodeName(SemiLinearSubGraph<IStoryNode> graph, string name)
    {
        return graph.NodeSequence.Any(s => s.Name.IsEqv(name));
    }
}