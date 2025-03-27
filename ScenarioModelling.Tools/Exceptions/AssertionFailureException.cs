using System.Text;

namespace ScenarioModelling.Tools.Exceptions;
public class AssertionFailureException : Exception
{
    public AssertionFailureException(string assertionExpression, string assertionName = "") : base(BuildAssertionMessage(assertionExpression, assertionName))
    {
    }

    private static string BuildAssertionMessage(string assertionExpression, string assertionName)
    {
        StringBuilder sb = new();

        if (string.IsNullOrWhiteSpace(assertionName))
        {
            sb.Append($"An assertion with expression <");
            sb.Append(assertionExpression);
            sb.Append($"> failed.");
        }
        else
        {
            sb.Append("The assertion ");
            sb.Append(assertionName);
            sb.Append(" with expression <");
            sb.Append(assertionExpression);
            sb.Append($"> failed.");
        }

        return sb.ToString();
    }
}
