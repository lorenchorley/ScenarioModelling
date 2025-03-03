using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Execution.Dialog;

public class ParallelConstructionExecutor : DialogExecutor
{
    private readonly MetaStoryDefinitionStack _metaStoryDefinitionStack;
    private readonly EventGenerationDependencies _dependencies;

    public ParallelConstructionExecutor(Context context, MetaStoryDefinitionStack metaStoryDefinitionStack, IServiceProvider serviceProvider, EventGenerationDependencies dependencies) : base(context/*, metaStoryStack*/, serviceProvider)
    {
        _metaStoryDefinitionStack = metaStoryDefinitionStack;
        _dependencies = dependencies;
    }

    public override void StartMetaStory(string name)
    {
        base.StartMetaStory(name);

        var node = _story!.NextNode();
    }

    protected override void InitStory()
    {
        if (_metaStory == null || _story == null)
            throw new ExecutionException("MetaStory not started");

        _story.Init(_metaStory/*, dontAddToStack: true*/);
    }

    internal void AddNodeToStoryAndAdvance(IStoryNode newNode)
    {
        ArgumentNullExceptionStandard.ThrowIfNull(_story);


        RegisterEvent(newNode.GenerateEvent(_dependencies));
        _story.CurrentScope.MoveToNextInSequence();
    }

    public IMetaStoryEvent GenerateEvent(IStoryNode node)
    {
        return node.GenerateEvent(_dependencies);
    }

}
