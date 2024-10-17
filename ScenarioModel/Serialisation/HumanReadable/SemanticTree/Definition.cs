namespace Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public abstract record Definition
{
    public Definition? ParentDefinition { get; init; }
}
