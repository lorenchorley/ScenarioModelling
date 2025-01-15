using ScenarioModelling.ContextValidation.Errors;

namespace ScenarioModelling.ContextValidation.Interfaces;

public interface INodeValidator
{

}

public interface INodeValidator<T> : INodeValidator
{
    ValidationErrors Validate(System system, MetaStory scenario, T instance);
}
