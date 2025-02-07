using System.Text;

namespace ScenarioModelling.Serialisation.HumanReadable.Reserialisation.WorkInProgress;

public class StrMkr
{
    private record ObjectSerialisationProfile
    {
        public List<ObjectSerialisationProfile> Profiles { get; } = new List<ObjectSerialisationProfile>();
        public StringBuilder StringBuilder { get; } = new();
    }

    private const int _indentLength = 2;
    private int _indentLevel = 0;
    private string _currentIndent = "";
    private ObjectSerialisationProfile _topProfile = new();
    private Stack<ObjectSerialisationProfile> _stack = new();

    public void Append(string text)
    {
        _stack.Peek().StringBuilder.AppendLine(text);
    }

    private void InitialiseObjectScope(ObjectSerialisationProfile instance)
    {

    }

    private void FinaliseObjectScope(ObjectSerialisationProfile instance)
    {

    }

    private void InitialiseIndentScope()
    {
        _indentLevel++;
        _currentIndent = new string(' ', _indentLevel * _indentLength);
    }

    private void FinaliseIndentScope()
    {
        _indentLevel--;
        _currentIndent = new string(' ', _indentLevel * _indentLength);
    }

    public IDisposable ObjectScope => new StrMkrScope<ObjectSerialisationProfile>(InitialiseObjectScope, FinaliseObjectScope, new());

    public IDisposable StartIndent => new StrMkrScope(InitialiseIndentScope, FinaliseIndentScope);

    public string GetFinalString()
    {
        return "";
    }
}

public class StrMkrScope<T> : IDisposable
{
    private readonly Action<T> _endScope;
    private readonly T _instance;

    public StrMkrScope(Action<T> startScope, Action<T> endScope, T instance)
    {
        _endScope = endScope;
        _instance = instance;

        startScope(instance);
    }

    public void Dispose() => _endScope(_instance);
}

public class StrMkrScope : IDisposable
{
    private readonly Action _endScope;

    public StrMkrScope(Action startScope, Action endScope)
    {
        _endScope = endScope;

        startScope();
    }

    public void Dispose() => _endScope();
}
