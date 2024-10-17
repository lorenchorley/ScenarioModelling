namespace Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public record ExpressionBlock
{
    public StringValue ExpressionText { get; init; } = null!;

    public override string ToString()
    {
        return $"<{ExpressionText}>";
    }
}
