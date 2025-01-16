using ScenarioModelling.ContextValidation.Errors;

namespace ScenarioModelling.ContextValidation;

public class ContextValidator
{
    public ValidationErrors Validate(Context context)
    {
        ValidationErrors validationErrors = new();
        MetaStoryValidator MetaStoryValidator = new();
        SystemValidator systemValidator = new();

        foreach (var MetaStory in context.MetaStories)
        {
            validationErrors.Incorporate(MetaStoryValidator.Validate(context.System, MetaStory));
        }

        validationErrors.Incorporate(systemValidator.Validate(context.System));

        return validationErrors;
    }
}
