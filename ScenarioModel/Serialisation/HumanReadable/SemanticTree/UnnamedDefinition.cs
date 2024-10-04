using YamlDotNet.Core.Tokens;

namespace Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public record UnnamedDefinition : Definition
{
    public IDValue Type { get; init; } = null!;
    public Definitions Definitions { get; init; } = new();

    public override string ToString()
    {
        if (Definitions.Count == 0)
        {
            return $"UnnamedDefinition({Type}, _)";
        }
        else
        {
            return $"UnnamedDefinition({Type}, _) {{ {Definitions} }}";
        }
    }
}
