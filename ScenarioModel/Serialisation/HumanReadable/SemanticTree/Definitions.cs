using LanguageExt.ClassInstances.Pred;
using System.Text;

namespace Isagri.Reporting.StimulSoftMigration.Quid.RequestFilters.SemanticTree;

public class Definitions : List<Definition>
{
    public override string ToString()
    {
        StringBuilder sb = new();

        for (int i = 0; i < Count; i++)
        {
            if (i > 0)
            {
                sb.Append(' ');
            }

            sb.Append(this[i].ToString());
        }

        return sb.ToString();
    }
}
