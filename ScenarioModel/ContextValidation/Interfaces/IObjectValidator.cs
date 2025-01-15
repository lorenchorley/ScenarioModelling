using ScenarioModelling.ContextValidation.Errors;

namespace ScenarioModelling.ContextValidation.Interfaces;

public interface IObjectValidator
{

}

public interface IObjectValidator<T> : IObjectValidator
{
    ValidationErrors Validate(System system, T instance);
}