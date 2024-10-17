namespace Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public record NamedLinkDefinition : UnnamedLinkDefinition
{
    public StringValue Name { get; init; } = null!;

    public override string ToString()
    {
        return $"Link({Source} -> {Destination}, {Name.Value})";
    }
}
