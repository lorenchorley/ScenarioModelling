using ScenarioModelling.CoreObjects.TestCaseNodes;
using System.Text;

namespace ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;

public class TestCaseSerialiser
{
    public void WriteTestCases(StringBuilder sb, List<TestCase> testCases, string currentIndent)
    {
        string innerIndent = currentIndent + CustomContextSerialiser.IndentSegment;

        foreach (var testCase in testCases)
        {
            sb.AppendLine($"{currentIndent}TestCase {{");

            if (!string.IsNullOrWhiteSpace(testCase.MetaStoryName))
            {
                sb.AppendLine($"{innerIndent}MetaStoryName {testCase.MetaStoryName}");
            }

            if (testCase.InitialStates.Count > 0)
            {
                sb.AppendLine($"{innerIndent}InitialStates {{");
                WriteStates(sb, testCase.InitialStates, innerIndent);
                sb.AppendLine($"{innerIndent}}}");
            }

            if (testCase.FinalStates.Count > 0)
            {
                sb.AppendLine($"{innerIndent}FinalStates {{");
                WriteStates(sb, testCase.FinalStates, innerIndent);
                sb.AppendLine($"{innerIndent}}}");
            }

            sb.AppendLine($"{currentIndent}}}");
        }
    }

    private void WriteStates(StringBuilder sb, Dictionary<string, string> states, string currentIndent)
    {
        foreach (var state in states)
        {
            sb.AppendLine($"{state.Key} {state.Value}");
        }
    }
}

