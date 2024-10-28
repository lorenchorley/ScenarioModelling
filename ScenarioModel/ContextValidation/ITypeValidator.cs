namespace ScenarioModel.Validation;

public interface ITypeValidator<T>
{
    ValidationErrors ValidateType(T instance);
}