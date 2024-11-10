using ScenarioModel.Collections;
using ScenarioModel.Objects.ScenarioNodes;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.Validation;

public class GraphValidator : IValidator<SemiLinearSubGraph<IScenarioNode>>
{
    public ValidationErrors Validate(SemiLinearSubGraph<IScenarioNode> graph)
    {
        ValidationErrors validationErrors = new();

        foreach (var node in graph.NodeSequence)
        {
            switch (node)
            {
                case ChooseNode chooseAction:

                    foreach (var choice in chooseAction.Choices)
                    {
                        validationErrors.AddIfNot(IsNodeName(graph, choice.NodeName), new InvalidNodeName($"Node {choice} not found on {typeof(ChooseNode)} {chooseAction.Name}"));
                    }

                    break;
            }
        }

        return validationErrors;
    }

    private bool IsNodeName(SemiLinearSubGraph<IScenarioNode> graph, string name)
    {
        return graph.NodeSequence.Any(s => s.Name.IsEqv(name));
    }
}