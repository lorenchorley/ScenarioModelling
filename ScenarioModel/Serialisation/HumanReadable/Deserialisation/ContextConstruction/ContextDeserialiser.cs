﻿using LanguageExt;
using LanguageExt.Common;
using ScenarioModel.ContextConstruction;
using ScenarioModel.Exhaustiveness;
using ScenarioModel.Expressions.Initialisation;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.References;
using ScenarioModel.References.Interfaces;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.NodeDeserialisers.Intefaces;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction.ObjectDeserialisers;
using ScenarioModel.Serialisation.HumanReadable.Deserialisation.IntermediateSemanticTree;
using Relation = ScenarioModel.Objects.SystemObjects.Relation;

namespace ScenarioModel.Serialisation.HumanReadable.Deserialisation.ContextConstruction;

public class ContextBuilderInputs : IContextBuilderInputs
{
    public List<Definition> DefinitionTreeTopLevel { get; } = new();
}

public class ContextDeserialiser : IContextBuilder<ContextBuilderInputs>
{
    private readonly Dictionary<string, IDefinitionToNodeDeserialiser> _nodeProfilesByName = new();
    private readonly Dictionary<Func<Definition, bool>, IDefinitionToNodeDeserialiser> _nodeProfilesByPredicate = new();

    private readonly Context _context;
    private readonly Instanciator _instanciator;
    private readonly RelationDeserialiser _relationTransformer;
    private readonly ConstraintDeserialiser _constraintTransformer;
    private readonly StateDeserialiser _stateTransformer;
    private readonly TransitionDeserialiser _transitionTransformer;
    private readonly AspectDeserialiser _aspectTransformer;
    private readonly StateMachineDeserialiser _stateMachineTransformer;
    private readonly EntityTypeDeserialiser _entityTypeTransformer;
    private readonly EntityDeserialiser _entityTransformer;
    private readonly ScenarioTransformer _scenarioTransformer;

    private readonly IfNodeDeserialiser _ifNodeDeserialiser = new();
    private readonly WhileNodeDeserialiser _whileNodeDeserialiser = new();

    // State
    private List<Definition> _remainingLast = new();

    public bool HasBeenUsedAlready { get; private set; } = false;

    public ContextDeserialiser()
    {
        ScenarioNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<IDefinitionToNodeDeserialiser>();

        ScenarioNodeExhaustivity.DoForEachNodeType(
            chooseNode: () => RegisterNodeProfile(new ChooseNodeDeserialiser()),
            dialogNode: () => RegisterNodeProfile(new DialogNodeDeserialiser()),
            ifNode: () => RegisterNodeProfile(_ifNodeDeserialiser),
            jumpNode: () => RegisterNodeProfile(new JumpNodeDeserialiser()),
            transitionNode: () => RegisterNodeProfile(new TransitionNodeDeserialiser()),
            whileNode: () => RegisterNodeProfile(_whileNodeDeserialiser)
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

    public Result<Context> BuildContextFromInputs(ContextBuilderInputs inputs)
    {
        if (HasBeenUsedAlready)
            throw new Exception("This instance of ContextBuilder has already been used.");

        // Transform all definitions into objects with references to other objects
        // One line for each type of definition that can exist at the top level of the definition tree (that is with a minimum of indentation)
        Transform(inputs);

        // Create objects from unresolvable references
        CreateObjectsFromUnresolvableReferences();

        // Validate all objects
        InitialiseObjects();

        // Check that all definitions have been transformed
        if (_remainingLast.Any())
            throw new Exception($"Unknown definitions not taken into account : {_remainingLast.CommaSeparatedList()}");

        HasBeenUsedAlready = true;

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
                    new EntityType(_context.System) { Name = r.Name, ExistanceOriginallyInferred = true };
                    break;
                case AspectReference r:
                    new Aspect(_context.System) { Name = r.Name };
                    break;
                //case AspectTypeReference r:
                //    new AspectType(_context.System) { Name = r.Name, ExistanceOriginallyInferred = true };
                //    break;
                case StateReference r:
                    new State(_context.System) { Name = r.Name };
                    break;
                case StateMachineReference r:
                    new StateMachine(_context.System) { Name = r.Name, ExistanceOriginallyInferred = true };
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
        var (entities, remaining1) = inputs.DefinitionTreeTopLevel.PartitionByChoose(_entityTransformer.TransformAsObject);
        var (entityTypes, remaining2) = remaining1.PartitionByChoose(_entityTypeTransformer.TransformAsObject);
        var (stateMachines, remaining3) = remaining2.PartitionByChoose(_stateMachineTransformer.TransformAsObject);
        var (constraints, remaining4) = remaining3.PartitionByChoose(_constraintTransformer.TransformAsObject);
        var (relations, remaining5) = remaining4.PartitionByChoose(_relationTransformer.TransformAsObject);
        var (scenarios, remainingLast) = remaining5.PartitionByChoose(_scenarioTransformer.TransformAsObject);

        _context.Scenarios.AddRange(scenarios);
        _remainingLast.AddRange(remainingLast);
    }

    public void InitialiseObjects()
    {
        SystemObjectExhaustivity.DoForEachObjectType(
            entity: () => _entityTransformer.BeforeIndividualInitialisation(),
            entityType: () => _entityTypeTransformer.BeforeIndividualInitialisation(),
            aspect: () => _aspectTransformer.BeforeIndividualValidation(),
            relation: () => _relationTransformer.BeforeIndividualInitialisation(),
            state: () => _stateTransformer.BeforeIndividualInitialisation(),
            stateMachine: () => _stateMachineTransformer.BeforeIndividualInitialisation(),
            transition: () => _transitionTransformer.BeforeIndividualInitialisation(),
            constraint: () => _constraintTransformer.BeforeIndividualInitialisation()
        );

        SystemObjectExhaustivity.DoForEachObjectType(
            entity: () => _context.System.Entities.ForEach(_entityTransformer.Initialise),
            entityType: () => _context.System.EntityTypes.ForEach(_entityTypeTransformer.Initialise),
            aspect: () => _context.System.Aspects.ForEach(_aspectTransformer.Validate),
            relation: () => _context.System.Relations.ForEach(_relationTransformer.Initialise),
            state: () => _context.System.States.ForEach(_stateTransformer.Initialise),
            stateMachine: () => _context.System.StateMachines.ForEach(_stateMachineTransformer.Initialise),
            transition: () => _context.System.Transitions.ForEach(_transitionTransformer.Initialise),
            constraint: () => _context.System.Constraints.ForEach(_constraintTransformer.Initialise)
        );

        _context.Scenarios.ForEach(_scenarioTransformer.Initialise);

        // Initialise expressions
        var nodes =
            Enumerable.Empty<IScenarioNodeWithExpression>()
                      .Concat(_ifNodeDeserialiser.ConditionsToInitialise)
                      .Concat(_whileNodeDeserialiser.ConditionsToInitialise);

        foreach (var node in nodes)
        {
            ExpressionInitialiser visitor = new(_context.System);
            node.Condition.Accept(visitor);

            if (visitor.Errors.Any())
            {
                throw new Exception($"Expression on if node{node.LineInformation} was not valid : \n" + visitor.Errors.CommaSeparatedList());
            }
        }

        _ifNodeDeserialiser.ConditionsToInitialise.Clear();
        _whileNodeDeserialiser.ConditionsToInitialise.Clear();

        // TODO check that this is done after
        // Check for unresolvable references and check for name uniqueness
        //_context.System.CheckForUnresolvableReferences(); // Normalement fait juste après
        //_context.System.CheckForNameUniquenessByType(); // Normalement fait juste après
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

    private void RegisterNodeProfile(IDefinitionToNodeDeserialiser profile)
    {
        if (!string.IsNullOrEmpty(profile.Name))
            _nodeProfilesByName.Add(profile.Name, profile);

        if (profile.Predicate != null)
            _nodeProfilesByPredicate.Add(profile.Predicate, profile);
    }

}