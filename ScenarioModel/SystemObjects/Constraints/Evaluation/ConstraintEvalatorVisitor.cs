using LanguageExt;
using ScenarioModel.SystemObjects.Constraints.Traversal;
using ScenarioModel.SystemObjects.Entities;
using Relation = ScenarioModel.SystemObjects.Relations.Relation;

namespace ScenarioModel.SystemObjects.Constraints.Evaluation;

public class ConstraintEvalatorVisitor : IConstraintNodeVisitor
{
    private readonly System _system;
    private bool _currentEvaluationResult;

    public ConstraintEvalatorVisitor(System system)
    {
        _system = system;
    }

    public object VisitAndConstraint(AndConstraint andConstraint)
    {
        var leftResult = (bool)andConstraint.Left.Accept(this);
        var rightResult = (bool)andConstraint.Right.Accept(this);

        return leftResult && rightResult;
    }

    public object VisitOrConstraint(OrConstraint orConstraint)
    {
        var leftResult = (bool)orConstraint.Left.Accept(this);
        var rightResult = (bool)orConstraint.Right.Accept(this);

        return leftResult || rightResult;
    }

    public object VisitHasRelationConstraint(HasRelationConstraint hasRelationConstraint)
    {
        Option<Relation> systemRelation = hasRelationConstraint.Ref.ResolveReference(_system);

        if (systemRelation.IsNone)
        {
            return false;
        }

        var relatables = hasRelationConstraint.RelatableObject.ResolveReference(_system);

        return relatables.Match(
            relatable => { 
                foreach (var objectRelation in relatable.Relations)
                {
                    if (systemRelation.Case == objectRelation)
                    {
                        return true;
                    }
                }

                return false;
            },
            () => false
        );
    }

}
