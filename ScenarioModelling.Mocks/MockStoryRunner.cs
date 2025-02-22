using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.Execution;
using ScenarioModelling.Execution.Events;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Mocks;

public class MockStoryRunner
{
    public MockDialogExecutor Executor { get; }
    public EventGenerationDependencies Dependencies { get; }
    public Dictionary<string, Queue<string>>? ChoicesByNodeName { get; }

    public MockStoryRunner(MockDialogExecutor executor, EventGenerationDependencies dependencies, Dictionary<string, Queue<string>>? choicesByNodeName = null)
    {
        Executor = executor;
        Dependencies = dependencies;
        ChoicesByNodeName = choicesByNodeName;
    }

    public Story Run(string metaStoryName)
    {
        // Initialize the MetaStory
        Executor.StartMetaStory(metaStoryName);

        // Generate first node
        IStoryNode? node = null;

        while ((node = Executor.NextNode()) != null)
        {
            IMetaStoryEvent e = node.GenerateEvent(Dependencies);

            // Custom test context behaviour
            DoCustomRunBehaviour(ChoicesByNodeName, node, e);

            Executor.RegisterEvent(e);
        }

        return Executor.EndMetaStory();
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

    internal Dictionary<string, string> GetFinalStatesByEntityName()
    {
        return Executor.Context
                       .MetaState
                       .Entities
                       .Where(e => e.State.IsSet)
                       .ToDictionary(e => e.Name, e => e.State.ResolvedValue!.Name);
    }

}
