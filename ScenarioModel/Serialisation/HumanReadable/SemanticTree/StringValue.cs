using System.Text;

namespace Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public record StringValue
{
    public string Value { get; init; } = null!;

    public override string ToString()
    {
        return Value;
    }
}
