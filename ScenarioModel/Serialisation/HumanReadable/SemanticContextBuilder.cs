using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using LanguageExt;
using LanguageExt.Common;
using ScenarioModel.Expressions.SemanticTree;
using ScenarioModel.ScenarioObjects;
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

    private interface IStepProfile
    {
        string Name { get; }
        IScenarioNode CreateAndConfigure(UnnamedDefinition def, Scenario scenario);
    }

    private class ChooseStepProfile : IStepProfile
    {
        public string Name => "Choose";

        public IScenarioNode CreateAndConfigure(UnnamedDefinition def, Scenario scenario)
        {
            ChooseNode node = new();

            foreach (var item in def.Definitions)
            {
                if (item is UnnamedDefinition unnamed)
                {
                    node.Choices.Add(unnamed.Type.Value);
                }
            }

            return node;
        }
    }

    private class DialogStepProfile : IStepProfile
    {
        public string Name => "Dialog";

        public IScenarioNode CreateAndConfigure(UnnamedDefinition def, Scenario scenario)
        {
            DialogNode node = new();

            foreach (var item in def.Definitions)
            {
                if (item is NamedDefinition named)
                {
                    if (named.Type.Value.IsEqv("Text"))
                    {
                        node.TextTemplate = named.Name.Value;
                        continue;
                    }
                    
                    if (named.Type.Value.IsEqv("Character") || named.Type.Value.IsEqv("Char"))
                    {
                        node.TextTemplate = named.Name.Value;
                        continue;
                    }
                }
            }

            return node;
        }
    }

    private class StateTransitionStepProfile : IStepProfile
    {
        public string Name => "Transition";

        public IScenarioNode CreateAndConfigure(UnnamedDefinition def, Scenario scenario)
        {
            StateTransitionNode node = new();

            foreach (var item in def.Definitions)
            {
                if (item is TransitionDefinition transitionDefinition)
                {
                    node.TransitionName = transitionDefinition.TransitionName.Value;

                    IStateful? stateful = scenario.System.AllStateful.FirstOrDefault(e => e.Name.IsEqv(transitionDefinition.Type));
                    if (stateful != null)
                    {
                        node.StatefulObject = stateful.GenerateReference();
                        break;
                    }
                }
            }

            if (node.StatefulObject == null)
            {
                throw new Exception("Stateful object not set on transition node");
            }

            return node;
        }
    }
    
    private class JumpStepProfile : IStepProfile
    {
        public string Name => "Jump";

        public IScenarioNode CreateAndConfigure(UnnamedDefinition def, Scenario scenario)
        {
            JumpNode node = new();

            foreach (var item in def.Definitions)
            {
                if (item is UnnamedDefinition unnamedDefinition)
                {
                    node.Target = unnamedDefinition.Type.Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(node.Target))
            {
                throw new Exception("Target not set on jump node");
            }

            return node;
        }
    }

    private readonly Dictionary<string, IStepProfile> _stepTypes = new();

    public SemanticContextBuilder()
    {
        RegisterStepProfile(new ChooseStepProfile());
        RegisterStepProfile(new DialogStepProfile());
        RegisterStepProfile(new StateTransitionStepProfile());
        RegisterStepProfile(new JumpStepProfile());
    }

    public Result<Context> Build(List<Definition> tree)
    {
        Context context = Context.New();

        context.System.Entities.AddRange(tree.Choose(def => TransformEntity(def, context)));
        context.System.EntityTypes.AddRange(tree.Choose(TransformEntityType));
        context.System.StateMachines.AddRange(tree.Choose(TransformStateMachine));
        context.System.Constraints.AddRange(tree.Choose(TransformConstraint));
        context.Scenarios.AddRange(tree.Choose(TransformScenario(context.System)));

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

    private void RegisterStepProfile(IStepProfile profile)
    {
        _stepTypes.Add(profile.Name, profile);
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

            value.Steps.AddRange(named.Definitions.Choose(TransformStep(value)));

            //Console.WriteLine($"Created Scenario {value.Name}");

            return value;
        };

    private Func<Definition, Option<IScenarioNode>> TransformStep(Scenario scenario)
        => (Definition definition) =>
        {
            if (definition is not UnnamedDefinition unnamed)
            {
                return null;
            }

            if (!_stepTypes.TryGetValue(unnamed.Type.Value, out IStepProfile? profile) || profile == null)
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

        Entity value = new()
        {
            Relations = unnamed.Definitions.Choose(TransformRelation).ToList(),
            State = unnamed.Definitions.Choose(TransformState).FirstOrDefault(),
            EntityType = type,
        };

        value.Aspects = unnamed.Definitions.Choose(TransformAspect(value)).ToList();

        SetNameOrRecordForAutoNaming(definition, value, _entityNamer);

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