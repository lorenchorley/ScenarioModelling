using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using LanguageExt;
using LanguageExt.Common;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.States;
using System.Xml.Linq;

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
        IScenarioNode CreateAndConfigure(UnnamedDefinition def);
    }

    private class ChooseStepProfile : IStepProfile
    {
        public string Name => "Choose";

        public IScenarioNode CreateAndConfigure(UnnamedDefinition def)
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

        public IScenarioNode CreateAndConfigure(UnnamedDefinition def)
        {
            DialogNode node = new();

            foreach (var item in def.Definitions)
            {
                if (item is NamedDefinition named && named.Type.Value == "Text")
                {
                    node.TextTemplate = named.Name.Value;
                }
            }

            return node;
        }
    }
    
    private class StateTransitionStepProfile : IStepProfile
    {
        public string Name => "Transition";

        public IScenarioNode CreateAndConfigure(UnnamedDefinition def)
        {
            StateTransitionNode node = new();

            foreach (var item in def.Definitions)
            {
                //if (item is UnnamedLinkDefinition link)
                //{
                //    dialogNode.Dialog.Add(link.Link);
                //}
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
    }

    public Result<Context> Build(List<Definition> tree)
    {
        Context context = Context.New();

        context.System.Entities.AddRange(tree.Choose(def => TransformEntity(def, context)));
        context.System.EntityTypes.AddRange(tree.Choose(TransformEntityType));
        context.System.StateMachines.AddRange(tree.Choose(TransformStateType));
        context.System.Constraints.AddRange(tree.Choose(TransformConstraint));
        context.Scenarios.AddRange(tree.Choose(TransformScenario(context.System)));
        
        context.System.Entities.ForEach(ValdiateEntity);
        context.System.StateMachines.ForEach(ValidateStateMachine);

        _entityNamer.NameUnnamedObjects(context.System.Entities);
        _entityTypeNamer.NameUnnamedObjects(context.System.EntityTypes);
        _aspectTypeNamer.NameUnnamedObjects(context.System.AspectTypes);
        _stateTypeNamer.NameUnnamedObjects(context.System.StateMachines);
        _relationNamer.NameUnnamedObjects(context.System.AllRelations);
        _stateNamer.NameUnnamedObjects(context.System.AllStates);

        return context;
    }

    private void ValdiateEntity(Entity entity)
    {
        entity.EntityType ??= _entityTypeNamer.AddUnnamedDefinition(new EntityType());
    }

    private void ValidateStateMachine(StateType type)
    {
        foreach (var state in type.States)
        {
            state.StateType = type;
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

            var step = profile.CreateAndConfigure(unnamed);

            SetNameOrRecordForAutoNaming(definition, step, _entityNamer);

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
            StateType = unnamed.Definitions.Choose(TransformStateType).FirstOrDefault(),
        };

        SetNameOrRecordForAutoNaming(definition, value, _entityTypeNamer);

        return value;
    }

    private Option<StateType> TransformStateType(Definition definition)
    {
        if (definition is not UnnamedDefinition unnamed)
        {
            return null;
        }

        if (!unnamed.Type.Value.Equals("SM", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        StateType value = new()
        {
            States = unnamed.Definitions.Choose(TransformState).ToList()
        };

        SetNameOrRecordForAutoNaming(definition, value, _stateTypeNamer);

        foreach (var subdef in unnamed.Definitions)
        {
            if (subdef is UnnamedLinkDefinition transition)
            {

            }
        }

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
            StateType = unnamed.Definitions.Choose(TransformStateType).FirstOrDefault() ?? _stateTypeNamer.AddUnnamedDefinition(new StateType())
        };

        SetNameOrRecordForAutoNaming(definition, value, _aspectTypeNamer);

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
            StateType = unnamed.Definitions.Choose(TransformStateType).FirstOrDefault() ?? _stateTypeNamer.AddUnnamedDefinition(new StateType())
        };

        SetNameOrRecordForAutoNaming(definition, value, _stateNamer);

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

    private Option<ConstraintExpression> TransformConstraint(Definition definition)
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