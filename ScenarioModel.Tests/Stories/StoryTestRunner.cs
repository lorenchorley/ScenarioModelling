using FluentAssertions;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.StoryNodes.DataClasses;
using System.Data;

namespace ScenarioModelling.Tests.Stories;

public class StoryTestRunner(DialogExecutor executor, EventGenerationDependencies dependencies, Dictionary<string, Queue<string>>? choicesByNodeName = null)
{
    public Story Run(string MetaStoryName)
    {
        // Initialize the MetaStory
        var MetaStoryRun = executor.StartMetaStory(MetaStoryName);

        // Generate first node
        IStoryNode? node = null;

        while ((node = executor.NextNode()) != null)
        {
            IStoryEvent e = node.GenerateGenericTypeEvent(dependencies);

            // Custom test context behaviour
            DoCustomRunBehaviour(choicesByNodeName, node, e);

            executor.RegisterEvent(e);
        }

        return MetaStoryRun;
    }

    private static void DoCustomRunBehaviour(Dictionary<string, Queue<string>>? choicesByNodeName, IStoryNode? node, IStoryEvent e)
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
