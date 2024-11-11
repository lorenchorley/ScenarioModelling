using LanguageExt;
using LanguageExt.Common;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.Interfaces;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeTransformers;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

using Relation = ScenarioModel.Objects.SystemObjects.Relation;

namespace ScenarioModel.Serialisation.HumanReadable;

public class ContextBuilderInputs : IContextBuilderInputs
{
    public List<Definition> TopLevelOfDefinitionTree { get; } = new();
}

public class DeserialisationContextBuilder : IContextBuilder<ContextBuilderInputs>
{
    private readonly Dictionary<string, IDefinitionToNodeTransformer> _nodeProfilesByName = new();
    private readonly Dictionary<Func<Definition, bool>, IDefinitionToNodeTransformer> _nodeProfilesByPredicate = new();

    private readonly Context _context;
    private readonly Instanciator _instanciator;
    private readonly RelationTransformer _relationTransformer;
    private readonly ConstraintTransformer _constraintTransformer;
    private readonly StateTransformer _stateTransformer;
    private readonly TransitionTransformer _transitionTransformer;
    private readonly AspectTransformer _aspectTransformer;
    private readonly StateMachineTransformer _stateMachineTransformer;
    private readonly EntityTypeTransformer _entityTypeTransformer;
    private readonly EntityTransformer _entityTransformer;
    private readonly ScenarioTransformer _scenarioTransformer;

    // State
    private List<Definition> _remainingLast = new();

    public bool IsUsed { get; private set; } = false;

    public DeserialisationContextBuilder()
    {
        NodeExhaustiveness.AssertExhaustivelyImplemented<IDefinitionToNodeTransformer>();

        NodeExhaustiveness.DoForEachNodeType(
            chooseNode: () => RegisterNodeProfile(new ChooseNodeProfile()),
            dialogNode: () => RegisterNodeProfile(new DialogNodeProfile()),
            ifNode: () => RegisterNodeProfile(new IfNodeProfile()),
            jumpNode: () => RegisterNodeProfile(new JumpNodeProfile()),
            stateTransitionNode: () => RegisterNodeProfile(new StateTransitionNodeProfile()),
            whileNode: () => RegisterNodeProfile(new WhileNodeProfile())
        );

        _context = Context.New();
        _instanciator = new(_context.System);
        _relationTransformer = new(_context.System, _instanciator);
        _constraintTransformer = new(_context.System, _instanciator);
        _stateTransformer = new(_context.System, _instanciator);
        _transitionTransformer = new(_context.System, _instanciator);
        _aspectTransformer = new(_context.System, _instanciator, _stateTransformer, _relationTransformer);
        _stateMachineTransformer = new(_context.System, _instanciator, _stateTransformer, _transitionTransformer);
        _entityTypeTransformer = new(_context.System, _instanciator, _stateMachineTransformer);
        _entityTransformer = new(_context.System, _instanciator, _stateTransformer, _aspectTransformer, _entityTypeTransformer, _relationTransformer);
        _scenarioTransformer = new(_context.System, _instanciator)
        {
            NodeProfilesByName = _nodeProfilesByName,
            NodeProfilesByPredicate = _nodeProfilesByPredicate
        };
    }

    public Result<Context> Build(ContextBuilderInputs inputs)
    {
        if (IsUsed)
            throw new Exception("This instance of ContextBuilder has already been used.");

        // Transform all definitions into objects with references to other objects
        // One line for each type of definition that can exist at the top level of the definition tree (that is with a minimum of indentation)
        Transform(inputs);

        // Create objects from unresolvable references
        CreateObjectsFromUnresolvableReferences();

        // Name all unnamed objects
        //NameUnnamedObjects(); // Done as we go

        // Validate all objects
        ValidateObjects();

        // Check that all definitions have been transformed
        if (_remainingLast.Any())
            throw new Exception($"Unknown definitions not taken into account : {_remainingLast.CommaSeparatedList()}");

        IsUsed = true;

        return _context;
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

    public void Transform(ContextBuilderInputs inputs)
    {
        var (entities, remaining1) = inputs.TopLevelOfDefinitionTree.PartitionByChoose(_entityTransformer.TransformAsObject);
        var (entityTypes, remaining2) = remaining1.PartitionByChoose(_entityTypeTransformer.TransformAsObject);
        var (stateMachines, remaining3) = remaining2.PartitionByChoose(_stateMachineTransformer.TransformAsObject);
        var (constraints, remaining4) = remaining3.PartitionByChoose(_constraintTransformer.TransformAsObject);
        var (relations, remaining5) = remaining4.PartitionByChoose(_relationTransformer.TransformAsObject);
        var (scenarios, remainingLast) = remaining5.PartitionByChoose(_scenarioTransformer.TransformAsObject);
        _context.Scenarios.AddRange(scenarios);

        _remainingLast.AddRange(remainingLast);
    }

    //public void NameUnnamedObjects()
    //{
    //    var allIdentifiable =
    //        Enumerable.Empty<IIdentifiable>()
    //                  .Concat(_context.System.Entities)
    //                  .Concat(_context.System.EntityTypes)
    //                  .Concat(_context.System.States)
    //                  .Concat(_context.System.StateMachines)
    //                  .Concat(_context.System.Transitions)
    //                  .Concat(_context.System.Aspects)
    //                  .Concat(_context.System.Relations)
    //                  .Concat(_context.System.Constraints);
    //    // TODO
    //}

    public void ValidateObjects()
    {
        _context.System.Entities.ForEach(_entityTransformer.Validate);
        _context.System.EntityTypes.ForEach(_entityTypeTransformer.Validate);
        _context.System.AllAspects.ToList().ForEach(_aspectTransformer.Validate);
        _context.System.StateMachines.ForEach(_stateMachineTransformer.Validate);
        _context.System.States.ForEach(_stateTransformer.Validate);
        _context.System.Transitions.ForEach(_transitionTransformer.Validate);
        _context.System.Relations.ForEach(_relationTransformer.Validate);
        _context.System.Constraints.ForEach(_constraintTransformer.Validate);
        _context.Scenarios.ForEach(_scenarioTransformer.Validate);

        // Check for unresolvable references and check for name uniqueness
        _context.System.CheckForUnresolvableReferences(); // Normalement fait juste après
        _context.System.CheckForNameUniquenessByType(); // Normalement fait juste après
    }

    #region Transformation

    private Option<AspectType> TransformAspectType(System system, Definition definition)
    {
        if (definition is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.Equals("AspectType", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        throw new NotImplementedException("TransformAspectType");

        //AspectType value = new(system)
        //{
        //    StateMachine = unnamed.Definitions.Choose(d => TransformStateMachine(system, d)).FirstOrDefault() ?? _stateMachineNamer.AddUnnamedDefinition(new StateMachine(system))
        //};

        ////SetNameOrRecordForAutoNaming(definition, value, _aspectTypeNamer);

        //return value;
    }

    #endregion

    #region Validation
    private void ValdiateEntity(Entity entity, Context context)
    {
        //entity.EntityType ??= _entityTypeNamer.AddUnnamedDefinition(new EntityType(context.System));

        //if (entity.EntityType.StateMachine == null)
        //{
        //    // We have to create the state present on the entity if it doesn't exist in the state machine for coherence
        //    string? stateName = entity.State.StateName;

        //    if (!string.IsNullOrEmpty(stateName))
        //    {
        //        StateMachine? stateMachine = context.System.StateMachines.FirstOrDefault(sm => sm.States.Any(s => s.Name.IsEqv(stateName)));
        //        entity.EntityType.StateMachine = stateMachine;
        //        //entity.State.StateMachine = stateMachine ?? entity.State.StateMachine;

        //        if (stateMachine != null)
        //        {
        //            // Use the state instance on the state machine so there aren't several instances floating around
        //            entity.State.SetValue(stateMachine.States.First(s => s.Name.IsEqv(stateName)));
        //        }
        //    }
        //}
    }

    private void ValdiateEntityType(EntityType entityType, Context context)
    {

    }

    private void ValidateAspect(Aspect aspect, Context context)
    {

    }

    private void ValidateStateMachine(StateMachine sm, Context context)
    {

    }

    private void ValidateState(State state, Context context)
    {
        if (state.StateMachine == null)
        {
            //foreach (var sm in context.System.StateMachines)
            //{
            //    if (sm.States.Any(s => s.Name.IsEqv(state.Name)))
            //    {
            //        state.StateMachine = sm;
            //        break;
            //    }
            //}

            // If the state machine on the state is still not set after searching through everything in the system, then make a new one
            //if (state.StateMachine == null)
            //{
            //    state.StateMachine = _stateMachineNamer.AddUnnamedDefinition(new StateMachine(context.System));
            //}

            //ValidateStateMachineStates(state.StateMachine, context);
            //ValidateTransition(state.StateMachine, context);
        }
    }

    #endregion

    private void RegisterNodeProfile(IDefinitionToNodeTransformer profile)
    {
        if (!string.IsNullOrEmpty(profile.Name))
            _nodeProfilesByName.Add(profile.Name, profile);

        if (profile.Predicate != null)
            _nodeProfilesByPredicate.Add(profile.Predicate, profile);
    }

}