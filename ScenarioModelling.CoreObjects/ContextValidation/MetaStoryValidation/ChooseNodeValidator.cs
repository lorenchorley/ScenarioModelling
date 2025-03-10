using ScenarioModelling.Annotations.Attributes;
using ScenarioModelling.CoreObjects.ContextValidation.Errors;
using ScenarioModelling.CoreObjects.ContextValidation.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CoreObjects.ContextValidation.MetaStoryValidation;

[StoryNodeLike<INodeValidator, ChooseNode>]
public class ChooseNodeValidator : INodeValidator<ChooseNode>
{
    public ValidationErrors Validate(MetaState system, MetaStory MetaStory, ChooseNode node)
    {
        ValidationErrors validationErrors = new();

        foreach (var choice in node.Choices)
        {
            //validationErrors.AddIfNot(IsNodeName(graph, choice.NodeName), new InvalidNodeName($"Node {choice} not found on {typeof(ChooseNode)} {node.Name}"));
        }

        return validationErrors;
    }

    //private bool IsNodeName(SemiLinearSubGraph<IStoryNode> graph, string name)
    //{
    //    return graph.NodeSequence.Any(s => s.Name.IsEqv(name));
    //}
}