namespace ScenarioModel.ContextValidation.Errors;

public class ValidationErrors : List<ValidationError>
{
    public void AddIf(bool condition, ValidationError error)
    {
        if (condition)
        {
            Add(error);
        }
    }

    public void AddIfNot(bool condition, ValidationError error)
    {
        if (!condition)
        {
            Add(error);
        }
    }

    public void Incorporate(ValidationErrors otherErrors)
    {
        AddRange(otherErrors);
    }

    public override string ToString()
    {
        return string.Join("\n", this.Select(a => a.Message));
    }
}