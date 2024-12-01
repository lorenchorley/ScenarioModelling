using LanguageExt.Common;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;

namespace ScenarioModel.CodeHooks;

public class HookContextBuilderInputs : IContextBuilderInputs
{
    public Queue<object> NewObjects { get; } = new();
    public Queue<IScenarioNode> NewNodes { get; } = new();

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

    public Result<Context> BuildContextFromInputs(HookContextBuilderInputs inputs)
    {
        // Transform all definitions into objects with references to other objects
        // One line for each type of definition that can exist at the top level of the definition tree (that is with a minimum of indentation)
        Transform(inputs);

        // Create objects from unresolvable references
        CreateObjectsFromUnresolvableReferences();

        // Validate all objects
        InitialiseObjects();

        // Reinitialize the inputs so that everything is ready to be reused
        inputs.Reset();

        return _context;
    }

    public void Transform(HookContextBuilderInputs inputs)
    {
        while (inputs.NewObjects.TryDequeue(out var obj))
        {

        }

        while (inputs.NewNodes.TryDequeue(out var node))
        {

        }
    }

    public void CreateObjectsFromUnresolvableReferences()
    {
        var allIdentifiable =
            Enumerable.Empty<IReference>()
                      .Concat(_context.System.AllEntityReferences)
                      .Concat(_context.System.AllEntityTypeReferences)
                      .Concat(_context.System.AllStateReferences)
                      .Concat(_context.System.AllStateMachineReferences)
                      .Concat(_context.System.AllTransitionReferences)
                      .Concat(_context.System.AllAspectReferences)
                      .Concat(_context.System.AllRelationReferences)
                      .Concat(_context.System.AllConstraintReferences);

        foreach (var reference in allIdentifiable)
        {
            if (reference.IsResolvable())
                continue;

            switch (reference) // TODO Exhaustivity ?
            {
                case EntityReference r:
                    new Entity(_context.System) { Name = r.Name };
                    break;
                case EntityTypeReference r:
                    new EntityType(_context.System) { Name = r.Name };
                    break;
                case AspectReference r:
                    new Aspect(_context.System) { Name = r.Name };
                    break;
                case StateReference r:
                    new State(_context.System) { Name = r.Name };
                    break;
                case StateMachineReference r:
                    new StateMachine(_context.System) { Name = r.Name };
                    break;
                case TransitionReference r:
                    new Transition(_context.System) { Name = r.Name };
                    break;
                case RelationReference r:
                    new Relation(_context.System) { Name = r.Name };
                    break;
                case ConstraintReference r:
                    new Constraint(_context.System) { Name = r.Name };
                    break;
                default:
                    throw new NotImplementedException($"Reference type {reference.GetType().Name} not implemented.");
            }
        }
    }

    public void InitialiseObjects()
    {

    }
}