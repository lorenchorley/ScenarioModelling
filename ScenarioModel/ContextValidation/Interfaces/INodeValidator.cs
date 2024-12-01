using ScenarioModel.ContextValidation.Errors;

namespace ScenarioModel.ContextValidation.Interfaces;

public interface INodeValidator
{

}

public interface INodeValidator<T> : INodeValidator
{
    ValidationErrors Validate(System system, Scenario scenario, T instance);
}
