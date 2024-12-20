﻿using ScenarioModel.CodeHooks.HookDefinitions;

namespace ScenarioModel.CodeHooks;

public class ScenarioHookOrchestratorForValidation(Context context) : ScenarioHookOrchestrator(context)
{
    public override ScenarioHookDefinition? DeclareScenarioStart(string name)
    {
        if (Context.Scenarios.Any(s => s.Name == name))
        {
            throw new InvalidOperationException($"Scenario name {name} does not exist in context");
        }

        return base.DeclareScenarioStart(name);
    }
}