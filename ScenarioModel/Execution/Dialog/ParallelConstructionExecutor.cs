using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Expressions.Evaluation;
using ScenarioModelling.Interpolation;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;
using ScenarioModelling.Objects.StoryNodes.DataClasses;

namespace ScenarioModelling.Execution.Dialog;

public class ParallelConstructionExecutor : DialogExecutor
{
    private readonly EventGenerationDependencies _dependencies;

    public ParallelConstructionExecutor(Context context, ExpressionEvalator evalator) : base(context, evalator)
    {
        DialogExecutor executor = new(context, evalator);
        StringInterpolator interpolator = new(context.System);
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

        RegisterEvent(newNode.GenerateGenericTypeEvent(_dependencies));
        var nextNode = _story.CurrentScope.GetNextInSequence();
        //if (nextNode != newNode)
        //{
        //    throw new InvalidOperationException($"The story being constructed in parallel to hook execution was not in phase. Expected next node to be {newNode}, but was {nextNode}");
        //}
    }

    internal IStoryEvent GenerateEvent(IStoryNode node)
    {
        return node.GenerateGenericTypeEvent(_dependencies);
    }

}
