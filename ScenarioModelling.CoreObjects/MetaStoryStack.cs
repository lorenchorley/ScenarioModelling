namespace ScenarioModelling.CoreObjects;

public class MetaStoryStack
{
    public int Count => _stack.Count;

    private readonly Stack<MetaStory> _stack = new();

    public void Push(MetaStory metaStory)
        => _stack.Push(metaStory);

    public MetaStory Pop()
        => _stack.Pop();

    public MetaStory Peek()
        => _stack.Peek();
}
