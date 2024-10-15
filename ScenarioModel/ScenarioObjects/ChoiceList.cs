namespace ScenarioModel.ScenarioObjects;

public class ChoiceList : List<string>
{
    public override string ToString()
    {
        return string.Join(", ", this);
    }
}