using System.Data;
using System.Text;

namespace ScenarioModelling.Interpolation;
public class StringInterpolator(System System)
{
    public string ReplaceInterpolations(string str)
    {
        if (str.IndexOf('{') == -1)
        {
            return str;
        }

        var sb = new StringBuilder();

        ScanForStart(sb, str);

        return sb.ToString();
    }

    private void ScanForStart(StringBuilder sb, string substr)
    {
        int index = substr.IndexOf('{');

        if (index == -1)
        {
            sb.Append(substr);
            return;
        }

        sb.Append(substr.Substring(0, index));

        //ScanForEnd(sb, substr[(index + 1)..]);
        ScanForEnd(sb, substr.Substring(index + 1));
    }

    private void ScanForEnd(StringBuilder sb, string substr)
    {
        int index = substr.IndexOf('}');

        if (index == -1)
        {
            throw new SyntaxErrorException("No closing brace found");
        }

        string expression = substr.Substring(0, index);

        sb.Append(Interpolate(expression));

        //ScanForStart(sb, substr[(index + 1)..]);
        ScanForStart(sb, substr.Substring(index + 1));
    }

    private string Interpolate(string expression)
    {
        return InterpretAsDottedExpression(System, expression);
    }

    private static string InterpretAsDottedExpression(System System, string expression)
    {
        string[] split = expression.Split('.');

        if (split.Length != 2)
        {
            throw new SyntaxErrorException("Invalid expression");
        }

        string entityName = split[0];
        string propertyName = split[1];

        if (propertyName == "State")
        {
            var entity = System.AllStateful.FirstOrDefault(e => e.Name == entityName);

            if (entity == null)
            {
                throw new SyntaxErrorException($"Entity not found: {entityName}");
            }

            return entity.State.ResolvedValue?.Name ?? "Not set";
        }

        throw new Exception($"Property not found: {propertyName}");
    }
}
