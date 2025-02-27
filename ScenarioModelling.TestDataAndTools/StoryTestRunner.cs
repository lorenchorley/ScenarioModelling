using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Dialog;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Execution.Events.Interfaces;
using System.Data;

namespace ScenarioModelling.TestDataAndTools;

public class StoryTestRunner(DialogExecutor executor, EventGenerationDependencies dependencies, Dictionary<string, Queue<string>>? choicesByNodeName = null)
{
    public Story Run(string metaStoryName)
    {
        // Initialize the MetaStory
        executor.StartMetaStory(metaStoryName);

        // Generate first node
        IStoryNode? node = null;

        while ((node = executor.NextNode()) != null)
        {
            IMetaStoryEvent e = node.GenerateEvent(dependencies);

            // Custom test context behaviour
            DoCustomRunBehaviour(choicesByNodeName, node, e);

            executor.RegisterEvent(e);
        }

        return executor.EndMetaStory();
    }

    private static void DoCustomRunBehaviour(Dictionary<string, Queue<string>>? choicesByNodeName, IStoryNode? node, IMetaStoryEvent e)
    {
        node.ToOneOf().Switch(
            callMetaStory => { },
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
            metadataNode => { },
            transitionNode => { },
            whileNode => { }
        );
    }
}
