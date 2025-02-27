using LanguageExt;
using LanguageExt.Common;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.CoreObjects.Expressions.Initialisation;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Exhaustiveness;
using ScenarioModelling.Serialisation.ContextConstruction;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.StoryNodeDeserialisers.Intefaces;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction.SystemObjectDeserialisers;
using ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.IntermediateSemanticTree;
using System.Diagnostics;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Deserialisation.ContextConstruction;

public class ContextBuilderInputs : IContextBuilderInputs
{
    public List<Definition> TopLevelOfDefinitionTree { get; } = new();
}

public class CustomContextDeserialiser : IContextBuilder<ContextBuilderInputs>
{
    private readonly Dictionary<string, IDefinitionToNodeDeserialiser> _nodeProfilesByName = new();
    private readonly Dictionary<Func<Definition, bool>, IDefinitionToNodeDeserialiser> _nodeProfilesByPredicate = new();

    private readonly Context _context;
    private readonly Instanciator _instanciator;

    private readonly MetaStoryTransformer _metaStoryTransformer;

    private readonly RelationDeserialiser _relationTransformer;
    private readonly ConstraintDeserialiser _constraintTransformer;
    private readonly StateDeserialiser _stateTransformer;
    private readonly TransitionDeserialiser _transitionTransformer;
    private readonly AspectDeserialiser _aspectTransformer;
    private readonly StateMachineDeserialiser _stateMachineTransformer;
    private readonly EntityTypeDeserialiser _entityTypeTransformer;
    private readonly EntityDeserialiser _entityTransformer;

    private readonly ChooseNodeDeserialiser _chooseNodeDeserialiser;
    private readonly DialogNodeDeserialiser _dialogNodeDeserialiser;
    private readonly JumpNodeDeserialiser _jumpNodeDeserialiser;
    private readonly MetadataNodeDeserialiser _metadataNodeDeserialiser;
    private readonly TransitionNodeDeserialiser _transitionNodeDeserialiser;
    private readonly IfNodeDeserialiser _ifNodeDeserialiser;
    private readonly WhileNodeDeserialiser _whileNodeDeserialiser;

    // State
    private List<Definition> _remainingLast = new();

    public bool HasBeenUsedAlready { get; private set; } = false;

    public CustomContextDeserialiser(
        Context context,
        Instanciator instanciator,
        RelationDeserialiser relationTransformer,
        ConstraintDeserialiser constraintTransformer,
        StateDeserialiser stateTransformer,
        TransitionDeserialiser transitionTransformer,
        AspectDeserialiser aspectTransformer,
        StateMachineDeserialiser stateMachineTransformer,
        EntityTypeDeserialiser entityTypeTransformer,
        EntityDeserialiser entityTransformer,
        MetaStoryTransformer metaStoryTransformer,
        CallMetaStoryNodeDeserialiser callMetaStoryNodeDeserialiser,
        ChooseNodeDeserialiser chooseNodeDeserialiser,
        DialogNodeDeserialiser dialogNodeDeserialiser,
        JumpNodeDeserialiser jumpNodeDeserialiser,
        MetadataNodeDeserialiser metadataNodeDeserialiser,
        TransitionNodeDeserialiser transitionNodeDeserialiser,
        IfNodeDeserialiser ifNodeDeserialiser,
        WhileNodeDeserialiser whileNodeDeserialiser
        )
    {
        //MetaStoryNodeExhaustivity.AssertInterfaceExhaustivelyImplemented<IDefinitionToNodeDeserialiser>();

        MetaStoryNodeExhaustivity.DoForEachNodeType(
            callMetaStory: () => RegisterNodeProfile(callMetaStoryNodeDeserialiser),
            chooseNode: () => RegisterNodeProfile(chooseNodeDeserialiser),
            dialogNode: () => RegisterNodeProfile(dialogNodeDeserialiser),
            ifNode: () => RegisterNodeProfile(ifNodeDeserialiser),
            jumpNode: () => RegisterNodeProfile(jumpNodeDeserialiser),
            metadataNode: () => RegisterNodeProfile(metadataNodeDeserialiser),
            transitionNode: () => RegisterNodeProfile(transitionNodeDeserialiser),
            whileNode: () => RegisterNodeProfile(whileNodeDeserialiser)
        );

        _context = context;
        _instanciator = instanciator;
        _relationTransformer = relationTransformer;
        _constraintTransformer = constraintTransformer;
        _stateTransformer = stateTransformer;
        _transitionTransformer = transitionTransformer;
        _aspectTransformer = aspectTransformer;
        _stateMachineTransformer = stateMachineTransformer;
        _entityTypeTransformer = entityTypeTransformer;
        _entityTransformer = entityTransformer;
        _chooseNodeDeserialiser = chooseNodeDeserialiser;
        _dialogNodeDeserialiser = dialogNodeDeserialiser;
        _jumpNodeDeserialiser = jumpNodeDeserialiser;
        _metadataNodeDeserialiser = metadataNodeDeserialiser;
        _transitionNodeDeserialiser = transitionNodeDeserialiser;
        _ifNodeDeserialiser = ifNodeDeserialiser;
        _whileNodeDeserialiser = whileNodeDeserialiser;

        _metaStoryTransformer = metaStoryTransformer;
        _metaStoryTransformer.NodeProfilesByName = _nodeProfilesByName;
        _metaStoryTransformer.NodeProfilesByPredicate = _nodeProfilesByPredicate;
    }

    public Result<Context> RefreshContextWithInputs(ContextBuilderInputs inputs)
    {
        if (HasBeenUsedAlready)
            throw new Exception("This instance of ContextBuilder has already been used.");

        // Transform all definitions into objects with references to other objects
        // One line for each type of definition that can exist at the top level of the definition tree (that is with a minimum of indentation)
        Transform(inputs);

        CheckForUntransformedInputs(inputs);

        // Create objects from unresolvable references
        _context.CreateObjectsFromUnresolvableReferences();

        // Validate all objects
        InitialiseObjects();

        // Check that all definitions have been transformed
        if (_remainingLast.Any())
            throw new Exception($"Unknown definitions not taken into account : {_remainingLast.CommaSeparatedList()}");

        HasBeenUsedAlready = true;

        return _context;
    }

    public void Transform(ContextBuilderInputs inputs)
    {
        _remainingLast.Clear();

        var (entities, remaining1) = inputs.TopLevelOfDefinitionTree.PartitionByChoose(_entityTransformer.TransformAsObject);
        var (entityTypes, remaining2) = remaining1.PartitionByChoose(_entityTypeTransformer.TransformAsObject);
        var (stateMachines, remaining3) = remaining2.PartitionByChoose(_stateMachineTransformer.TransformAsObject);
        var (constraints, remaining4) = remaining3.PartitionByChoose(_constraintTransformer.TransformAsObject);
        var (relations, remaining5) = remaining4.PartitionByChoose(_relationTransformer.TransformAsObject);
        var (metaStories, remainingLast) = remaining5.PartitionByChoose(_metaStoryTransformer.TransformAsObject);

        _remainingLast.AddRange(remainingLast);
    }

    public void CheckForUntransformedInputs(ContextBuilderInputs inputs)
    {
        List<string> untransformedDefinitionPaths = new();

        CheckForUntransformedDefinitions(inputs.TopLevelOfDefinitionTree, untransformedDefinitionPaths, "");

        if (untransformedDefinitionPaths.Count > 0)
        {
            StringBuilder sb = new();

            sb.AppendLine("The following deserialised definitions were not used in the building of the context :");
            foreach (var untransformedDefinitionPath in untransformedDefinitionPaths)
            {
                sb.AppendLine(untransformedDefinitionPath);
            }

            Debug.WriteLine(sb.ToString());
            //throw new InternalLogicException(sb.ToString());
        }
    }

    public void CheckForUntransformedDefinitions(List<Definition> definitions, List<string> untransformedDefinitionPaths, string path)
    {
        foreach (Definition definition in definitions)
        {
            if (definition is ParentalDefinition parentalDefinition)
                CheckForUntransformedDefinitions(parentalDefinition.Definitions, untransformedDefinitionPaths, $"{path} -> {definition.ToEssentialString()}");

            if (!definition.HasBeenTransformed)
                untransformedDefinitionPaths.Add($"{path} -> {definition.ToEssentialString()}");
        }
    }

    public void InitialiseObjects()
    {
        MetaStateObjectExhaustivity.DoForEachObjectType(
            entity: () => _entityTransformer.BeforeIndividualInitialisation(),
            entityType: () => _entityTypeTransformer.BeforeIndividualInitialisation(),
            aspect: () => _aspectTransformer.BeforeIndividualValidation(),
            relation: () => _relationTransformer.BeforeIndividualInitialisation(),
            state: () => _stateTransformer.BeforeIndividualInitialisation(),
            stateMachine: () => _stateMachineTransformer.BeforeIndividualInitialisation(),
            transition: () => _transitionTransformer.BeforeIndividualInitialisation(),
            constraint: () => _constraintTransformer.BeforeIndividualInitialisation()
        );

        MetaStateObjectExhaustivity.DoForEachObjectType(
            entity: () => _context.MetaState.Entities.ForEach(_entityTransformer.Initialise),
            entityType: () => _context.MetaState.EntityTypes.ForEach(_entityTypeTransformer.Initialise),
            aspect: () => _context.MetaState.Aspects.ForEach(_aspectTransformer.Validate),
            relation: () => _context.MetaState.Relations.ForEach(_relationTransformer.Initialise),
            state: () => _context.MetaState.States.ForEach(_stateTransformer.Initialise),
            stateMachine: () => _context.MetaState.StateMachines.ForEach(_stateMachineTransformer.Initialise),
            transition: () => _context.MetaState.Transitions.ForEach(_transitionTransformer.Initialise),
            constraint: () => _context.MetaState.Constraints.ForEach(_constraintTransformer.Initialise)
        );

        _context.MetaStories.ForEach(_metaStoryTransformer.Initialise);

        // Initialise expressions
        var nodes =
            Enumerable.Empty<IStoryNodeWithExpression>()
                      .Concat(_ifNodeDeserialiser.ConditionsToInitialise)
                      .Concat(_whileNodeDeserialiser.ConditionsToInitialise);

        foreach (var node in nodes)
        {
            ExpressionInitialiser visitor = new(_context.MetaState);
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

    private void RegisterNodeProfile(IDefinitionToNodeDeserialiser profile)
    {
        if (!string.IsNullOrEmpty(profile.Name))
            _nodeProfilesByName.Add(profile.Name, profile);

        if (profile.Predicate != null)
            _nodeProfilesByPredicate.Add(profile.Predicate, profile);
    }

}