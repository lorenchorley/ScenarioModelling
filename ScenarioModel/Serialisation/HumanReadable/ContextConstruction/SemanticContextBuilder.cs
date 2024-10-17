using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using LanguageExt;
using LanguageExt.Common;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.ScenarioObjects;
using ScenarioModel.Serialisation.HumanReadable.ContextConstruction;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Serialisation.HumanReadable;

public class SemanticContextBuilder
{
    private readonly DefinitionAutoNamer _entityNamer = new("E");
    private readonly DefinitionAutoNamer _entityTypeNamer = new("ET");
    private readonly DefinitionAutoNamer _aspectTypeNamer = new("AT");
    private readonly DefinitionAutoNamer _stateTypeNamer = new("SM");
    private readonly DefinitionAutoNamer _relationNamer = new("R");
    private readonly DefinitionAutoNamer _stateNamer = new("S");

    private readonly Dictionary<string, ISemanticStepProfile> _stepsByName = new();
    private readonly Dictionary<Func<Definition, bool>, ISemanticStepProfile> _stepsByPredicate = new();

    public SemanticContextBuilder()
    {
        RegisterStepProfile(new ChooseStepProfile());
        RegisterStepProfile(new DialogStepProfile());
        RegisterStepProfile(new StateTransitionStepProfile());
        RegisterStepProfile(new JumpStepProfile());
        RegisterStepProfile(new IfStepProfile());
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

        var (scenarios, remaining5) = remaining4.PartitionByChoose(TransformScenario(context.System));
        context.Scenarios.AddRange(scenarios);

        if (remaining5.Any())
        {
            throw new Exception($"Unknown definitions not taken into account : {remaining5.CommaSeparatedList()}");
        }

        context.System.StateMachines.ForEach(sm => ValidateStateMachineStates(sm, context));
        context.System.Entities.ForEach(e => ValdiateEntity(e, context));
        context.System.AllStateful.ToList().ForEach(s => ValidateState(s, context));
        context.System.StateMachines.ForEach(sm => ValidateStateMachineTransitions(sm, context));

        _entityNamer.NameUnnamedObjects(context.System.Entities);
        _entityTypeNamer.NameUnnamedObjects(context.System.EntityTypes);
        _aspectTypeNamer.NameUnnamedObjects(context.System.AspectTypes);
        _stateTypeNamer.NameUnnamedObjects(context.System.StateMachines);
        _relationNamer.NameUnnamedObjects(context.System.AllRelations);
        _stateNamer.NameUnnamedObjects(context.System.AllStates);

        return context;
    }

    private void ValdiateEntity(Entity entity, Context context)
    {
        entity.EntityType ??= _entityTypeNamer.AddUnnamedDefinition(new EntityType());

        //Console.WriteLine($"Created EntityType {entity.EntityType.Name}");

        if (entity.EntityType.StateType == null)
        {
            if (entity.State != null)
            {
                StateMachine? stateMachine = context.System.StateMachines.FirstOrDefault(sm => sm.States.Any(s => s.Name.IsEqv(entity.State.Name)));
                entity.EntityType.StateType = stateMachine;
                entity.State.StateMachine = stateMachine ?? entity.State.StateMachine;
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
                state.StateMachine = _stateTypeNamer.AddUnnamedDefinition(new StateMachine());

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

    private void RegisterStepProfile(ISemanticStepProfile profile)
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

            value.Steps.AddRange(named.Definitions.ChooseAndAssertAllSelected(TransformStep(value), "Unknown step types not taken into account : {0}"));

            return value;
        };

    private Func<Definition, Option<IScenarioNode>> TransformStep(Scenario scenario)
        => (Definition definition) =>
        {
            foreach (var profilePred in _stepsByPredicate)
            {
                if (profilePred.Key(definition))
                {
                    IScenarioNode value = profilePred.Value.CreateAndConfigure(definition, scenario);
                    return Option<IScenarioNode>.Some(value);
                }
            }

            if (definition is not UnnamedDefinition unnamed)
            {
                return null;
            }

            if (!_stepsByName.TryGetValue(unnamed.Type.Value, out ISemanticStepProfile? profile) || profile == null)
            {
                throw new Exception("Unknown step type"); // TODO better
            }

            var step = profile.CreateAndConfigure(unnamed, scenario);

            SetNameOrRecordForAutoNaming(definition, step, _entityNamer);

            //Console.WriteLine($"Created Step {step.Name}");

            return Option<IScenarioNode>.Some(step);
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
            StateType = unnamed.Definitions.Choose(TransformStateMachine).FirstOrDefault(),
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

        SetNameOrRecordForAutoNaming(definition, value, _stateTypeNamer);

        foreach (var subdef in unnamed.Definitions)
        {
            if (subdef is NamedLinkDefinition transition)
            {
                Transition item = new()
                {
                    SourceState = transition.Source.Value,
                    DestinationState = transition.Destination.Value,
                    Name = transition.Name.Value
                };

                //Console.WriteLine($"Created Transition {item.Name}");

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

        AspectType value = new()
        {
            StateType = unnamed.Definitions.Choose(TransformStateMachine).FirstOrDefault() ?? _stateTypeNamer.AddUnnamedDefinition(new StateMachine())
        };

        SetNameOrRecordForAutoNaming(definition, value, _aspectTypeNamer);

        //Console.WriteLine($"Created AspectType {value.Name}");

        return value;
    }

    private Func<Definition, Option<Aspect>> TransformAspect(Entity entity)
        => (Definition definition) =>
        {
            if (definition is not UnnamedDefinition unnamed)
            {
                return null;
            }

            if (!unnamed.Type.Value.Equals("Aspect", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            Aspect value = new()
            {
                Entity = entity,
                Relations = unnamed.Definitions.Choose(TransformRelation).ToList(),
                State = unnamed.Definitions.Choose(TransformState).FirstOrDefault(),
                AspectType = unnamed.Definitions.Choose(TransformAspectType).FirstOrDefault() ?? _aspectTypeNamer.AddUnnamedDefinition(new AspectType())
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
            //StateType = unnamed.Definitions.Choose(TransformStateType).FirstOrDefault() ?? _stateTypeNamer.AddUnnamedDefinition(new StateType())
        };

        SetNameOrRecordForAutoNaming(definition, value, _stateNamer);

        //Console.WriteLine($"Created State {value.Name}");

        return value;
    }

    private Option<SystemObjects.Relations.Relation> TransformRelation(Definition definition)
    {
        if (definition is not UnnamedLinkDefinition unnamed)
        {
            return null;
        }

        return null;
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

}