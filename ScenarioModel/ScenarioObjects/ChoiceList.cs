namespace ScenarioModel.ScenarioObjects;

public class ChoiceList : List<(string NodeName, string Text)>
{
    public override string ToString()
    {
        return string.Join(", ", this);
    }
}