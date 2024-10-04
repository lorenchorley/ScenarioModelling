namespace Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public record IDValue
{
    public string Value { get; init; } = null!;

    public override string ToString()
    {
        return Value;
    }
}
