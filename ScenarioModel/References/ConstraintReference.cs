using LanguageExt;
using ScenarioModel.Objects.System.Constraints;

namespace ScenarioModel.References;

public record ConstraintReference : IReference<Constraint>
{
    public string ConstraintName { get; set; } = "";

    public Option<Constraint> ResolveReference(System system)
    {
        throw new NotImplementedException();
    }

    override public string ToString() => $"{ConstraintName}";
}
