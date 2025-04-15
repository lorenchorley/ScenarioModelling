using LanguageExt.Common;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Serialisation.ContextConstruction;

namespace ScenarioModelling.CodeHooks;

public class HookContextBuilder : IContextBuilder<Queue<IStoryNode>>
{
    private readonly Context _context;

    public HookContextBuilder(Context context)
    {
        _context = context;
    }

    public Result<Context> RefreshContextWithInputs(Queue<IStoryNode> inputs)
    {
        // Transform all definitions into objects with references to other objects
        // One line for each type of definition that can exist at the top level of the definition tree (that is with a minimum of indentation)
        Transform(inputs);

        // Create objects from unresolvable references
        _context.CreateObjectsFromUnresolvableReferences();

        // Validate all objects
        InitialiseObjects();

        // Reinitialize the inputs so that everything is ready to be reused
        inputs.Clear();

        return _context;
    }

    public void Transform(Queue<IStoryNode> inputs)
    {
        
    }

    public void InitialiseObjects()
    {

    }
}