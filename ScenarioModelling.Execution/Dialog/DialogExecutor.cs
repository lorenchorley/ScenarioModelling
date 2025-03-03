using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Execution.Events.Interfaces;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.Execution.Dialog;

public class DialogExecutor : IExecutor
{
    public Context Context { get; set; }

    protected MetaStory? _metaStory;
    protected Story? _story;
    //private readonly MetaStoryStack _metaStoryStack;
    private readonly IServiceProvider _serviceProvider;

    public DialogExecutor(Context context/*, MetaStoryStack metaStoryStack*/, IServiceProvider serviceProvider)
    {
        Context = context;
        //_metaStoryStack = metaStoryStack;
        _serviceProvider = serviceProvider;
    }

    public IMetaStoryEvent? GetLastEvent()
    {
        if (_metaStory == null || _story == null)
            throw new ExecutionException("MetaStory not started");

        return _story.Events.GetEnumerable().LastOrDefault();
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
            throw new ExecutionException($"No MetaStory with name {name}");

        _story = _serviceProvider.GetRequiredService<Story>();
        InitStory();
    }

    protected virtual void InitStory()
    {
        if (_metaStory == null || _story == null)
            throw new ExecutionException("MetaStory not started");

        _story.Init(_metaStory);
    }

    public Story EndMetaStory()
    {
        if (_metaStory == null || _story == null)
            throw new ExecutionException("MetaStory not started");

        // TODO Final validation that we are indend at the end of the metastory

        var story = _story;

        //if (_metaStoryStack.Count == 0)
        if (story.GraphScopeStack.Count == 0)
        {
            _story = null;
            _metaStory = null;
        }

        return story;
    }

    public IStoryNode? NextNode()
    {
        if (_metaStory == null || _story == null)
            throw new ExecutionException("MetaStory not started");

        return _story.NextNode();
    }

    public void RegisterEvent(IMetaStoryEvent @event)
    {
        if (_metaStory == null || _story == null)
            throw new ExecutionException("MetaStory not started");

        _story.RegisterEvent(@event);
    }

    public bool IsLastEventOfType<T>() where T : IMetaStoryEvent
    {
        return IsLastEventOfType<T>(_ => true);
    }

    public bool IsLastEventOfType<T>(Func<T, bool> pred) where T : IMetaStoryEvent
    {
        IMetaStoryEvent? MetaStoryEvent = _story?.Events.GetEnumerable().LastOrDefault();

        if (MetaStoryEvent == null)
        {
            return false;
        }

        return MetaStoryEvent is T t && pred(t);
    }
}
