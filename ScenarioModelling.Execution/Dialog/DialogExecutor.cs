using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Evaluation;
using ScenarioModelling.CoreObjects.StoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;

namespace ScenarioModelling.Execution.Dialog;

public class DialogExecutor : IExecutor
{
    public Context Context { get; set; }

    private MetaStory? _metaStory;
    protected Story? _story;
    private readonly ExpressionEvalator _evalator;

    public DialogExecutor(Context context, ExpressionEvalator evalator)
    {
        Context = context;
        _evalator = evalator;
    }

    public IStoryEvent? GetLastEvent()
    {
        if (_metaStory == null || _story == null)
            throw new InvalidOperationException("MetaStory not started");

        return _story.Events.LastOrDefault();
    }

    public void ResetToInitialState()
    {
        Context.ResetToInitialState();
    }

    public virtual void StartMetaStory(string name)
    {
        ResetToInitialState();

        _metaStory = Context.MetaStories.FirstOrDefault(s => s.Name.IsEqv(name));

        if (_metaStory == null)
            throw new InvalidOperationException($"No MetaStory with name {name}");

        _story = new Story { MetaStory = _metaStory, Evaluator = _evalator };
        _story.Init();
    }

    public Story EndMetaStory()
    {
        if (_metaStory == null || _story == null)
            throw new InvalidOperationException("MetaStory not started");

        // TODO Final validation that we are indend at the end of the metastory

        var story = _story;

        _story = null;
        _metaStory = null;

        return story;
    }

    public IStoryNode? NextNode()
    {
        if (_metaStory == null || _story == null)
            throw new InvalidOperationException("MetaStory not started");

        return _story.NextNode();
    }

    public void RegisterEvent(IStoryEvent @event)
    {
        if (_metaStory == null || _story == null)
            throw new InvalidOperationException("MetaStory not started");

        _story.RegisterEvent(@event);
    }

    public bool IsLastEventOfType<T>() where T : IStoryEvent
    {
        return IsLastEventOfType<T>(_ => true);
    }

    public bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IStoryEvent
    {
        IStoryEvent? MetaStoryEvent = _story?.Events.LastOrDefault();

        if (MetaStoryEvent == null)
        {
            return false;
        }

        return MetaStoryEvent is T t && pred(t);
    }
}
