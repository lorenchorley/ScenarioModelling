using LanguageExt;
using LanguageExt.Common;
using ScenarioModel.Collections;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.Objects.ScenarioObjects.BaseClasses;
using ScenarioModel.Objects.SystemObjects;
using ScenarioModel.Objects.SystemObjects.Entities;
using ScenarioModel.Objects.SystemObjects.States;
using ScenarioModel.References;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction.NodeProfiles;
using ScenarioModel.Serialisation.HumanReadable.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable;

public class SemanticContextBuilder
{
    private readonly DefinitionAutoNamer _entityNamer = new("E");
    private readonly DefinitionAutoNamer _entityTypeNamer = new("ET");
    private readonly DefinitionAutoNamer _aspectTypeNamer = new("AT");
    private readonly DefinitionAutoNamer _stateMachineNamer = new("SM");
    private readonly DefinitionAutoNamer _relationNamer = new("R");
    private readonly DefinitionAutoNamer _stateNamer = new("S");

    private readonly Dictionary<string, ISemanticNodeProfile> _stepsByName = new();
    private readonly Dictionary<Func<Definition, bool>, ISemanticNodeProfile> _stepsByPredicate = new();

    public SemanticContextBuilder()
    {
        RegisterStepProfile(new ChooseNodeProfile());
        RegisterStepProfile(new DialogNodeProfile());
        RegisterStepProfile(new StateTransitionNodeProfile());
        RegisterStepProfile(new JumpNodeProfile());
        RegisterStepProfile(new IfNodeProfile());
    }

    public Result<Context> Build(List<Definition> tree)
    {
        Context context = Context.New();

        var (entities, remaining1) = tree.PartitionByChoose(def => TransformEntity(def, context));
        context.System.Entities.AddRange(entities);

        var (entityTypes, remaining2) = remaining1.PartitionByChoose(TransformEntityType);
        context.System.EntityTypes.AddRange(entityTypes);

        var (stateMachines, remaining3) = remaining2.PartitionByChoose(TransformStateMachine);
        context.System.StateMachines.AddRange(stateMachines);

        var (constraints, remaining4) = remaining3.PartitionByChoose(TransformConstraint);
        context.System.Constraints.AddRange(constraints);

        var (relations, remaining5) = remaining4.PartitionByChoose(TransformRelation);
        context.System.TopLevelRelations.AddRange(relations);

        var (scenarios, remainingLast) = remaining5.PartitionByChoose(TransformScenario(context.System));
        context.Scenarios.AddRange(scenarios);

        if (remainingLast.Any())
        {
            throw new Exception($"Unknown definitions not taken into account : {remainingLast.CommaSeparatedList()}");
        }

        context.System.StateMachines.ForEach(sm => ValidateStateMachineStates(sm, context));
        context.System.Entities.ForEach(e => ValdiateEntity(e, context));
        context.System.AllStateful.ToList().ForEach(s => ValidateState(s, context));
        context.System.StateMachines.ForEach(sm => ValidateStateMachineTransitions(sm, context));

        _entityNamer.NameUnnamedObjects(context.System.Entities);
        _entityTypeNamer.NameUnnamedObjects(context.System.EntityTypes);
        _aspectTypeNamer.NameUnnamedObjects(context.System.AspectTypes);
        _stateMachineNamer.NameUnnamedObjects(context.System.StateMachines);
        _relationNamer.NameUnnamedObjects(context.System.AllRelations);
        _stateNamer.NameUnnamedObjects(context.System.AllStates);

        return context;
    }

    private void ValdiateEntity(Entity entity, Context context)
    {
        entity.EntityType ??= _entityTypeNamer.AddUnnamedDefinition(new EntityType());

        //Console.WriteLine($"Created EntityType {entity.EntityType.Name}");

        if (entity.EntityType.StateMachine == null)
        {
            if (entity.State != null)
            {
                StateMachine? stateMachine = context.System.StateMachines.FirstOrDefault(sm => sm.States.Any(s => s.Name.IsEqv(entity.State.Name)));
                entity.EntityType.StateMachine = stateMachine;
                //entity.State.StateMachine = stateMachine ?? entity.State.StateMachine;

                if (stateMachine != null)
                {
                    // Use the state instance on the state machine so there aren't several instances floating around
                    string stateName = entity.State.Name;
                    entity.State = stateMachine.States.First(s => s.Name.IsEqv(stateName));
                }
            }
        }
    }

    private void ValidateState(IStateful stateful, Context context)
    {
        State? state = stateful.State;

        if (state == null)
        {
            return;
        }

        if (state.StateMachine == null)
        {
            foreach (var sm in context.System.StateMachines)
            {
                if (sm.States.Any(s => s.Name.IsEqv(state.Name)))
                {
                    state.StateMachine = sm;
                    break;
                }
            }

            if (state.StateMachine == null)
            {
                state.StateMachine = _stateMachineNamer.AddUnnamedDefinition(new StateMachine());

                //Console.WriteLine($"Created StateMachine {state.StateMachine.Name}");
            }

            ValidateStateMachineStates(state.StateMachine, context);
            ValidateStateMachineTransitions(state.StateMachine, context);
        }
    }

    private void ValidateStateMachineStates(StateMachine type, Context context)
    {
        foreach (var transition in type.Transitions)
        {
            if (!type.States.Any(s => s.Name.IsEqv(transition.SourceState)))
            {
                State? state = context.System.AllStates.Where(s => s.Name.IsEqv(transition.SourceState)).FirstOrDefault();

                if (state == null)
                {
                    state = new State() { Name = transition.SourceState, StateMachine = type };

                    //Console.WriteLine($"Created State {state.Name}");
                }

                type.States.Add(state!);
            }

            if (!type.States.Any(s => s.Name.IsEqv(transition.DestinationState)))
            {
                var a = context.System.AllStates.ToList();
                State? state = context.System.AllStates.Where(s => s.Name.IsEqv(transition.DestinationState)).FirstOrDefault();

                if (state == null)
                {
                    state = new State() { Name = transition.DestinationState, StateMachine = type };

                    //Console.WriteLine($"Created State {state.Name}");
                }

                type.States.Add(state!);
            }
        }
    }

    private void ValidateStateMachineTransitions(StateMachine type, Context context)
    {
        foreach (var state in type.States)
        {
            state.StateMachine = type;

            var transitionsForState =
                type.Transitions.Where(t => t.SourceState.IsEqv(state.Name))
                    .ToList();

            foreach (var transition in transitionsForState)
            {
                if (!state.Transitions.Contains(transition))
                    state.Transitions.Add(transition);
            }
        }
    }

    private void RegisterStepProfile(ISemanticNodeProfile profile)
    {
        if (!string.IsNullOrEmpty(profile.Name))
            _stepsByName.Add(profile.Name, profile);

        if (profile.Predicate != null)
            _stepsByPredicate.Add(profile.Predicate, profile);
    }

    private Func<Definition, Option<Scenario>> TransformScenario(System system)
        => (Definition definition) =>
        {
            if (definition is not NamedDefinition named)
            {
                if (definition is not UnnamedDefinition unnamed)
                {
                    // Report error
                }

                return null;
            }

            if (!named.Type.Value.Equals("Scenario", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            Scenario value = new()
            {
                Name = named.Name.Value,
                System = system
            };

            var tryTransform = TryTransformDefinitionToNode(value);
            value.Graph.PrimarySubGraph.NodeSequence.AddRange(named.Definitions.ChooseAndAssertAllSelected(d => tryTransform(d, value.Graph.PrimarySubGraph), "Unknown step types not taken into account : {0}"));

            return value;
        };

    private Func<Definition, SemiLinearSubGraph<IScenarioNode>, Option<IScenarioNode>> TryTransformDefinitionToNode(Scenario scenario)
        => (Definition definition, SemiLinearSubGraph<IScenarioNode> currentSubgraph) =>
        {
            var tryTransform = TryTransformDefinitionToNode(scenario);

            foreach (var profilePred in _stepsByPredicate)
            {
                if (profilePred.Key(definition))
                {
                    var step = profilePred.Value.CreateAndConfigure(definition, scenario, currentSubgraph, tryTransform);
                    SetNameOrRecordForAutoNaming(definition, step, _entityNamer);
                    return Option<IScenarioNode>.Some(step);
                }
            }

            if (definition is ExpressionDefinition expDef)
            {
                if (_stepsByName.TryGetValue(expDef.Name.Value.ToUpperInvariant(), out ISemanticNodeProfile? profile) && profile != null)
                {
                    var step = profile.CreateAndConfigure(definition, scenario, currentSubgraph, tryTransform);
                    SetNameOrRecordForAutoNaming(definition, step, _entityNamer);
                    return Option<IScenarioNode>.Some(step);
                }
            }

            if (definition is UnnamedDefinition unnamed)
            {
                if (_stepsByName.TryGetValue(unnamed.Type.Value.ToUpperInvariant(), out ISemanticNodeProfile? profile) && profile != null)
                {
                    var step = profile.CreateAndConfigure(definition, scenario, currentSubgraph, tryTransform);
                    SetNameOrRecordForAutoNaming(definition, step, _entityNamer);
                    return Option<IScenarioNode>.Some(step);
                }
            }

            return null;
        };

    private Option<Entity> TransformEntity(Definition definition, Context context)
    {
        if (definition is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.Equals("Entity", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        EntityType? type = unnamed.Definitions.Choose(TransformEntityType).FirstOrDefault();
        if (type == null)
        {
            type = _entityTypeNamer.AddUnnamedDefinition(new EntityType());
            context.System.EntityTypes.Add(type);
        }

        var (relations, remaining1) = unnamed.Definitions.PartitionByChoose(TransformRelation);
        var (states, remaining2) = remaining1.PartitionByChoose(TransformState);


        Entity value = new()
        {
            Relations = relations.ToList(),
            State = states.FirstOrDefault(),
            EntityType = type,
        };

        value.Aspects = unnamed.Definitions.Choose(TransformAspect(value)).ToList();

        value.CharacterStyle = unnamed.Definitions.Choose(TransformCharacterStyle).FirstOrDefault() ?? "";

        SetNameOrRecordForAutoNaming(definition, value, _entityNamer);

        if (states.Count() > 1)
        {
            throw new Exception($"More than one state was set on entity {value.Name ?? "<unnamed>"} of type {type?.Name ?? "<unnnamed>"} : {states.Select(s => s.Name).CommaSeparatedList()}");
        }

        //Console.WriteLine($"Created Entity {value.Name}");

        return value;
    }

    private Option<EntityType> TransformEntityType(Definition definition)
    {
        if (definition is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.Equals("EntityType", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        EntityType value = new()
        {
            StateMachine = unnamed.Definitions.Choose(TransformStateMachine).FirstOrDefault(),
        };

        SetNameOrRecordForAutoNaming(definition, value, _entityTypeNamer);

        //Console.WriteLine($"Created EntityType {value.Name}");

        return value;
    }

    private Option<StateMachine> TransformStateMachine(Definition definition)
    {
        if (definition is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.Equals("SM", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        StateMachine value = new()
        {
            States = unnamed.Definitions.Choose(TransformState).ToList()
        };

        SetNameOrRecordForAutoNaming(definition, value, _stateMachineNamer);

        foreach (var subdef in unnamed.Definitions)
        {
            if (subdef is UnnamedLinkDefinition transition)
            {
                Transition item = new()
                {
                    SourceState = transition.Source.Value,
                    DestinationState = transition.Destination.Value,
                };

                //Console.WriteLine($"Created Transition {item.Name}");

                if (subdef is NamedLinkDefinition nammedTransition)
                {
                    item.Name = nammedTransition.Name.Value;
                }

                value.Transitions.Add(item);
            }
        }

        //Console.WriteLine($"Created StateMachine {value.Name}");

        return value;
    }

    private Option<AspectType> TransformAspectType(Definition definition)
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

        AspectType value = new()
        {
            StateMachine = unnamed.Definitions.Choose(TransformStateMachine).FirstOrDefault() ?? _stateMachineNamer.AddUnnamedDefinition(new StateMachine())
        };

        SetNameOrRecordForAutoNaming(definition, value, _aspectTypeNamer);

        //Console.WriteLine($"Created AspectType {value.Name}");

        return value;
    }

    private Func<Definition, Option<Aspect>> TransformAspect(Entity entity)
        => (Definition definition) =>
        {
            if (definition is not NamedDefinition named)
            {
                return null;
            }

            if (!named.Type.Value.Equals("Aspect", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            Aspect value = new()
            {
                Entity = entity,
                Relations = named.Definitions.Choose(TransformRelation).ToList(),
                State = named.Definitions.Choose(TransformState).FirstOrDefault(),
                AspectType = named.Definitions.Choose(TransformAspectType).FirstOrDefault() ?? new AspectType() { Name = named.Name.Value }
            };

            //Console.WriteLine($"Created Aspect {value.Name}");

            return value;
        };

    private Option<State> TransformState(Definition definition)
    {
        if (definition is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.Equals("State", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        State value = new()
        {
            //StateMachine = unnamed.Definitions.Choose(TransformStateMachine).FirstOrDefault() ?? _stateMachineNamer.AddUnnamedDefinition(new StateMachine())
        };

        SetNameOrRecordForAutoNaming(definition, value, _stateNamer);

        //Console.WriteLine($"Created State {value.Name}");

        return value;
    }

    private Option<ScenarioModel.Objects.SystemObjects.Relations.Relation> TransformRelation(Definition definition)
    {
        if (definition is not UnnamedLinkDefinition unnamed)
        {
            return null;
        }

        ScenarioModel.Objects.SystemObjects.Relations.Relation value = new()
        {
            LeftEntity = new RelatableObjectReference()
            {
                Identifier = new()
                {
                    ValueList = unnamed.Source.Value.Split('.').ToList()
                }
            },
            RightEntity = new RelatableObjectReference()
            {
                Identifier = new()
                {
                    ValueList = unnamed.Destination.Value.Split('.').ToList()
                }
            },
            // TODO State
        };

        SetLinkNameOrRecordForAutoNaming(definition, value, _stateNamer);

        return value;
    }

    private Option<string> TransformCharacterStyle(Definition definition)
    {
        if (definition is not NamedDefinition named)
        {
            return null;
        }

        if (named.Type.IsEqv("CharacterStyle"))
        {
            return named.Name.Value;
        }

        return null;
    }

    private Option<Expression> TransformConstraint(Definition definition)
    {
        return null;
    }

    private void SetNameOrRecordForAutoNaming(Definition definition, INameful value, DefinitionAutoNamer autoNamer)
    {
        if (definition is NamedDefinition named)
        {
            value.Name = named.Name.Value;
        }
        else
        {
            autoNamer.AddUnnamedDefinition(value);
        }
    }

    private void SetLinkNameOrRecordForAutoNaming(Definition definition, INameful value, DefinitionAutoNamer autoNamer)
    {
        if (definition is NamedLinkDefinition named)
        {
            value.Name = named.Name.Value;
        }
        else
        {
            autoNamer.AddUnnamedDefinition(value);
        }
    }

}