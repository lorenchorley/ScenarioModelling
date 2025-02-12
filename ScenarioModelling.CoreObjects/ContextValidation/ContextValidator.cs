using ScenarioModelling.CoreObjects.ContextValidation.Errors;

namespace ScenarioModelling.CoreObjects.ContextValidation;

public class ContextValidator
{
    private readonly MetaStoryValidator _metaStoryValidator;
    private readonly MetaStateValidator _systemValidator;

    public ContextValidator(MetaStoryValidator metaStoryValidator, MetaStateValidator systemValidator)
    {
        _metaStoryValidator = metaStoryValidator;
        _systemValidator = systemValidator;
    }

    public ValidationErrors Validate(Context context)
    {
        ValidationErrors validationErrors = new();

        foreach (var MetaStory in context.MetaStories)
        {
            validationErrors.Incorporate(_metaStoryValidator.Validate(context.MetaState, MetaStory));
        }

        validationErrors.Incorporate(_systemValidator.Validate(context.MetaState));

        return validationErrors;
    }
}
