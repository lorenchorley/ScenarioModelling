﻿using ScenarioModel.Collections;
using ScenarioModel.ScenarioObjects;

namespace ScenarioModel.Validation;

public class StepGraphValidator : IValidator<DirectedGraph<IScenarioNode>>
{
    public ValidationErrors Validate(DirectedGraph<IScenarioNode> stepGraph)
    {
        ValidationErrors validationErrors = new();

        foreach (var step in stepGraph)
        {
            switch (step)
            {
                case ChooseNode chooseAction:

                    foreach (var choice in chooseAction.Choices)
                    {
                        validationErrors.AddIfNot(IsStepName(stepGraph, choice.NodeName), new InvalidStepName($"Step {choice} not found on {typeof(ChooseNode)} {chooseAction.Name}"));
                    }

                    break;
            }
        }

        return validationErrors;
    }

    private bool IsStepName(DirectedGraph<IScenarioNode> stepGraph, string name)
    {
        return stepGraph.Any(s => s.Name.IsEqv(name));
    }
}