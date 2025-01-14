using FluentAssertions;
using ScenarioModel.Execution;
using ScenarioModel.Execution.Dialog;
using ScenarioModel.Execution.Events;
using ScenarioModel.Execution.Events.Interfaces;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.ScenarioNodes.DataClasses;
using System.Data;

namespace ScenarioModel.Tests.ScenarioRuns;

public class ScenarioTestRunner(DialogExecutor executor, EventGenerationDependencies dependencies, Dictionary<string, Queue<string>>? choicesByNodeName = null)
{
    public Story Run(string scenarioName)
    {
        // Initialize the scenario
        var scenarioRun = executor.StartScenario(scenarioName);

        // Generate first node
        IScenarioNode? node = null;

        while ((node = executor.NextNode()) != null)
        {
            IScenarioEvent e = node.GenerateGenericTypeEvent(dependencies);

            // Custom test context behaviour
            DoCustomRunBehaviour(choicesByNodeName, node, e);

            executor.RegisterEvent(e);
        }

        return scenarioRun;
    }

    private static void DoCustomRunBehaviour(Dictionary<string, Queue<string>>? choicesByNodeName, IScenarioNode? node, IScenarioEvent e)
    {
        node.ToOneOf().Switch(
            chooseNode =>
            {
                if (choicesByNodeName != null)
                {
                    if (!choicesByNodeName.TryGetValue(chooseNode.Name, out Queue<string>? choicesForNode) || choicesForNode == null)
                    {
                        throw new ArgumentException($"No choices queue provided for node {chooseNode.Name}, only for {choicesByNodeName.Keys.CommaSeparatedList()}");
                    }

                    string selection = choicesForNode.Dequeue();

                    ChoiceSelectedEvent choiceEvent = (ChoiceSelectedEvent)e;
                    choiceEvent.Choice =
                        chooseNode.Choices
                                    .Where(n => n.Text.IsEqv(selection))
                                    .Select(s => s.NodeName)
                                    .First();
                }
            },
            dialogNode => { },
            ifNode => { },
            jumpNode => { },
            transitionNode => { },
            whileNode => { }
        );
    }
}
