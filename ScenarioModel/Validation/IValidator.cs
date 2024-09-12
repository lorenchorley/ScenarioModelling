namespace ScenarioModel.Validation;

public interface IValidator<T>
{
    ValidationErrors Validate(T instance);
}