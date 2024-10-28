namespace ScenarioModel.Objects.Scenario;

public class ChoiceList : List<(string NodeName, string Text)>
{
    public override string ToString()
    {
        return this.CommaSeparatedList();
    }
}