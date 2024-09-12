using ScenarioModel.Collections;
using ScenarioModel.SystemObjects.Entities;

namespace ScenarioModel.Validation;

public class StepGraphValidator : IValidator<DirectedGraph<IScenarioAction>>
{
    public ValidationErrors Validate(DirectedGraph<IScenarioAction> stepGraph)
    {
        ValidationErrors validationErrors = new();

        foreach (var step in stepGraph)
        {
            switch (step)
            {
                case ChooseAction chooseAction:

                    foreach (var choice in chooseAction.Choices)
                    {
                        validationErrors.AddIfNot(IsStepName(stepGraph, choice), new InvalidStepName($"Step {choice} not found on {typeof(ChooseAction)} {chooseAction.Name}"));
                    }

                    break;
            }
        }

        return validationErrors;
    }

    private bool IsStepName(DirectedGraph<IScenarioAction> stepGraph, string name)
    {
        return stepGraph.Any(s => string.Equals(s.Name, s));
    }
}