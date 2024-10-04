using Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public class FilterParserResult : IParserResult<Definitions>
{
    public Definitions Tree { get; set; }

    public List<string> Errors { get; internal set; } = new();

    public bool HasErrors
    {
        get
        {
            return Errors.Count > 0;
        }
    }

    public override string ToString()
    {
        return Tree.ToString();
    }
}
