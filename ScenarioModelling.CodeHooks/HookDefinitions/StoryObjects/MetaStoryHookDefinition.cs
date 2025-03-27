using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks.HookDefinitions.StoryObjects;

public class MetaStoryHookDefinition
{
    public string Name { get; }
    public Context Context { get; }

    private MetaStory _associatedMetaStory;
    private readonly MetaStoryDefinitionStack _metaStoryStack;

    public MetaStoryHookDefinition(string name, Context context, MetaStoryDefinitionStack metaStoryStack)
    {
        Name = name;
        Context = context;
        _metaStoryStack = metaStoryStack;
        MetaStory? MetaStory = context.MetaStories.FirstOrDefault(s => s.Name == name);

        // If no existing meta is found, then we have to assume it's the first run through and create it
        if (MetaStory == null)
            MetaStory = context.NewMetaStory(name, new SemiLinearSubGraph<IStoryNode>());

        _associatedMetaStory = MetaStory;

        // This hook definition manages the instance of its meta story on the stack
        _metaStoryStack.Push(_associatedMetaStory);
    }

    public MetaStory EndMetaStory()
    {
        if (_metaStoryStack.Count == 0)
            throw new InternalLogicException("There was no meta story on the stack when the hook was called to end a meta story");

        // This hook definition manages the instance of its meta story on the stack
        MetaStory currentMetaStory = _metaStoryStack.Pop();

        if (currentMetaStory != _associatedMetaStory)
            throw new InternalLogicException("The meta story from the stack does not correspond to the associated meta story of the hook");

        return _associatedMetaStory;
    }


}
