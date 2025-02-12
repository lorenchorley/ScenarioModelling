using ScenarioModelling.CoreObjects.ContextValidation.Errors;

namespace ScenarioModelling.CoreObjects.ContextValidation.Interfaces;

public interface INodeValidator
{

}

public interface INodeValidator<T> : INodeValidator
{
    ValidationErrors Validate(MetaState system, MetaStory MetaStory, T instance);
}
