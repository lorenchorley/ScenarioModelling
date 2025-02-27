using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Execution.Dialog;

public class ParallelConstructionExecutor : DialogExecutor
{
    private readonly EventGenerationDependencies _dependencies;

    public ParallelConstructionExecutor(Context context, MetaStoryStack metaStoryStack, IServiceProvider serviceProvider, EventGenerationDependencies dependencies) : base(context, metaStoryStack, serviceProvider)
    {
        _dependencies = dependencies;
    }

    public override void StartMetaStory(string name)
    {
        base.StartMetaStory(name);

        var node = _story!.NextNode();
        if (node != null)
        {
            throw new Exception("Unexcepted initial node, should be null");
        }
    }

    protected override void InitStory()
    {
        if (_metaStory == null || _story == null)
            throw new ExecutionException("MetaStory not started");

        _story.Init(_metaStory, dontAddToStack: true);
    }

    internal void AddNodeToStoryAndAdvance(IStoryNode newNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_story);

        //_story!.CurrentScope.CurrentSubGraph.NodeSequence.Add(newNode);

        RegisterEvent(newNode.GenerateEvent(_dependencies));
        var nextNode = _story.CurrentScope.GetNextInSequence();
        //if (nextNode != newNode)
        //{
        //    throw new InvalidOperationException($"The story being constructed in parallel to hook execution was not in phase. Expected next node to be {newNode}, but was {nextNode}");
        //}
    }

    public IMetaStoryEvent GenerateEvent(IStoryNode node)
    {
        return node.GenerateEvent(_dependencies);
    }

}
