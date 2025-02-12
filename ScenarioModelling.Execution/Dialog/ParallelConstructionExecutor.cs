using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.Interpolation;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Dialog;

public class ParallelConstructionExecutor : DialogExecutor
{
    private readonly EventGenerationDependencies _dependencies;

    public ParallelConstructionExecutor(Context context, ExpressionEvalator evalator) : base(context, evalator)
    {
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.MetaState);
        _dependencies = new(interpolator, evalator, executor, context);
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

    public IStoryEvent GenerateEvent(IStoryNode node)
    {
        return node.GenerateEvent(_dependencies);
    }

}
