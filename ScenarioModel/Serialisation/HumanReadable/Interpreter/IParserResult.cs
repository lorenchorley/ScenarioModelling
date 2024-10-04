namespace ScenarioModel.Serialisation.HumanReadable.Interpreter;

public interface IParserResult<TNode>
{
    public TNode? Tree { get; }
    public List<string> Errors { get; }
    public bool HasErrors { get; }
}