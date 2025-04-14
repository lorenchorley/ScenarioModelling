using System.Runtime.CompilerServices;

namespace ScenarioModelling.Tools.Collections.Graph;

public interface ISubGraph<T> where T : IDirectedGraphNode<T>
{
    IEnumerable<T> UnorderedEnumerable { get; } 

    ISubGraphScope<T> GenerateScope(ISubGraphScope<T>? parentScope);

    bool Contains(T node);

    // TODO Add new methods so that NodeSequence doesn't need to be accessed directly
}
