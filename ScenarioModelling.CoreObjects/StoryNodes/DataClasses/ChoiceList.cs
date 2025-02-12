namespace ScenarioModelling.CoreObjects.StoryNodes.DataClasses;

public class ChoiceList : List<(string NodeName, string Text)>
{
    public bool IsEqv(ChoiceList other)
    {
        if (Count != other.Count)
            return false;

        for (int i = 0; i < Count; i++)
        {
            if (this[i].NodeName != other[i].NodeName)
                return false;

            if (this[i].Text != other[i].Text)
                return false;
        }

        return true;
    }

    public override string ToString()
    {
        return this.CommaSeparatedList();
    }
}