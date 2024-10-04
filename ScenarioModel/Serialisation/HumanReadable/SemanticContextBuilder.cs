using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;
using LanguageExt;
using LanguageExt.Common;
using ScenarioModel.SystemObjects.Entities;
using ScenarioModel.SystemObjects.States;

namespace ScenarioModel.Serialisation.HumanReadable;

internal class SemanticContextBuilder
{
    public Result<Context> Build(List<Definition> tree)
    {
        Context context = Context.New();

        context.System.Entities.AddRange(tree.Choose(TransformEntity));
        context.System.EntityTypes.AddRange(tree.Choose(TransformEntityType));
        context.System.StateTypes.AddRange(tree.Choose(TransformStateType));
        context.System.Constraints.AddRange(tree.Choose(TransformConstraint));

        return context;
    }

    private Option<Entity> TransformEntity(Definition definition)
    {
        return null;
    }

    private Option<EntityType> TransformEntityType(Definition definition)
    {
        return null;
    }

    private Option<StateType> TransformStateType(Definition definition)
    {
        return null;
    }

    private Option<Aspect> TransformAspect(Definition definition)
    {
        return null;
    }

    private Option<ConstraintExpression> TransformConstraint(Definition definition)
    {
        return null;
    }
}