using ScenarioModelling.ContextValidation.Errors;
using ScenarioModelling.ContextValidation.Interfaces;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Objects.ScenarioNodes;

namespace ScenarioModelling.ContextValidation.ScenarioValidation;

[NodeLike<INodeValidator, DialogNode>]
public class DialogNodeValidator : INodeValidator<DialogNode>
{
    public ValidationErrors Validate(System system, MetaStory scenario, DialogNode node)
    {
        ValidationErrors validationErrors = new();
        return validationErrors;
    }
}