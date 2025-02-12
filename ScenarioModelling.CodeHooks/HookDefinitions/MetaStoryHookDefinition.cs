using ScenarioModelling.CoreObjects;

namespace ScenarioModelling.CodeHooks.HookDefinitions;

public class MetaStoryHookDefinition
{
    public string Name { get; }
    public Context Context { get; }

    private MetaStory _activeMetaStory;

    public MetaStoryHookDefinition(string Name, Context Context)
    {
        this.Name = Name;
        this.Context = Context;

        MetaStory? MetaStory = Context.MetaStories.FirstOrDefault(s => s.Name == Name);

        if (MetaStory == null)
        {
            MetaStory = Context.NewMetaStory(Name);
        }

        _activeMetaStory = MetaStory;
    }

    public MetaStory GetMetaStory()
    {
        return _activeMetaStory;
    }
}
