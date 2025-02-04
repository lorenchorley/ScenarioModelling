using LanguageExt.Common;
using ScenarioModelling.ContextConstruction;
using ScenarioModelling.Objects.StoryNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks;

public class HookContextBuilderInputs : IContextBuilderInputs
{
    public Queue<object> NewObjects { get; } = new();
    public Queue<IStoryNode> NewNodes { get; } = new();

    public void Reset()
    {
        if (NewObjects.Count > 0)
            throw new Exception("New objects were not processed");

        if (NewNodes.Count > 0)
            throw new Exception("New nodes were not processed");
    }
}

public class ProgressiveHookBasedContextBuilder : IContextBuilder<HookContextBuilderInputs>
{
    private readonly Context _context;
    private readonly Instanciator _instanciator;

    public ProgressiveHookBasedContextBuilder(Context context)
    {
        _context = context;
        _instanciator = new(_context.System);
    }

    public Result<Context> RefreshContextWithInputs(HookContextBuilderInputs inputs)
    {

        // Transform all definitions into objects with references to other objects
        // One line for each type of definition that can exist at the top level of the definition tree (that is with a minimum of indentation)
        Transform(inputs);

        // Create objects from unresolvable references
        _context.CreateObjectsFromUnresolvableReferences();

        // Validate all objects
        InitialiseObjects();

        // Reinitialize the inputs so that everything is ready to be reused
        inputs.Reset();

        //_context.ResetToInitialState(); // TODO Too early !

        return _context;
    }

    public void Transform(HookContextBuilderInputs inputs)
    {
        while (inputs.NewObjects.Count > 0)
        {
            inputs.NewObjects.Dequeue();
        }

        while (inputs.NewNodes.Count > 0)
        {
            inputs.NewNodes.Dequeue();
        }
    }

    public void InitialiseObjects()
    {

    }
}