namespace Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public record NamedDefinition : UnnamedDefinition
{
    public StringValue Name { get; init; } = null!;

    public override string ToString()
    {
        if (Definitions.Count == 0)
        {
            return $"NamedDefinition({Type}, {Name})";
        }
        else
        {
            return $"NamedDefinition({Type}, {Name}) {{ {Definitions} }}";
        }
    }
}
