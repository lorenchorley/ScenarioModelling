using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Expressions.Evaluation;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.Execution.Dialog;

public class DialogExecutor : IExecutor
{
    public Context Context { get; set; }
    private MetaStory? _metaStory;
    private Story? _metaStoryRun;
    private readonly ExpressionEvalator _evalator;

    public DialogExecutor(Context context, ExpressionEvalator evalator)
    {
        Context = context;
        _evalator = evalator;
    }

    public IStoryEvent? GetLastEvent()
    {
        if (_metaStory == null || _metaStoryRun == null)
            throw new InvalidOperationException("MetaStory not started");

        return _metaStoryRun.Events.LastOrDefault();
    }

    public Story StartMetaStory(string name)
    {
        Context.ResetToInitialState();

        _metaStory = Context.MetaStorys.FirstOrDefault(s => s.Name == name);

        if (_metaStory == null)
            throw new InvalidOperationException($"No MetaStory with name {name}");

        _metaStoryRun = new Story { MetaStory = _metaStory, Evaluator = _evalator };
        _metaStoryRun.Init();

        return _metaStoryRun;
    }

    public IStoryNode? NextNode()
    {
        if (_metaStory == null || _metaStoryRun == null)
            throw new InvalidOperationException("MetaStory not started");

        return _metaStoryRun.NextNode();
    }

    public void RegisterEvent(IStoryEvent @event)
    {
        if (_metaStory == null || _metaStoryRun == null)
            throw new InvalidOperationException("MetaStory not started");

        _metaStoryRun.RegisterEvent(@event);
    }

    public bool IsLastEventOfType<T>() where T : IStoryEvent
    {
        return IsLastEventOfType<T>(_ => true);
    }

    public bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IStoryEvent
    {
        IStoryEvent? MetaStoryEvent = _metaStoryRun?.Events.LastOrDefault();

        if (MetaStoryEvent == null)
        {
            return false;
        }

        return MetaStoryEvent is T t && pred(t);
    }
}
