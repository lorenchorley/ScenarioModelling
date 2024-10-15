using YamlDotNet.Core.Tokens;

namespace Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public record UnnamedDefinition : Definition
{
    public StringValue Type { get; init; } = null!;
    public Definitions Definitions { get; init; } = new();

    public override string ToString()
    {
        if (Definitions.Count == 0)
        {
            return $"Definition({Type}, _)";
        }
        else
        {
            return $"Definition({Type}, _) {{ {Definitions} }}";
        }
    }
}
