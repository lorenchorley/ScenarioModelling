using ScenarioModelling.CoreObjects.ContextValidation.Errors;

namespace ScenarioModelling.CoreObjects.ContextValidation.Interfaces;

public interface IObjectValidator
{

}

public interface IObjectValidator<T> : IObjectValidator
{
    ValidationErrors Validate(MetaState system, T instance);
}