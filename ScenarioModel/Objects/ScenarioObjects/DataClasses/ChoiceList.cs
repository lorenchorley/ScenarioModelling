namespace ScenarioModel.Objects.ScenarioObjects.DataClasses;

public class ChoiceList : List<(string NodeName, string Text)>
{
    public override string ToString()
    {
        return this.CommaSeparatedList();
    }
}