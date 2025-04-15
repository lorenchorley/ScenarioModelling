using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Tools.Collections.Graph;

namespace ScenarioModelling.CodeHooks.Utils;

public class SubGraphScopeSnapshot
{
    public int Index { get; set; }
    public SemiLinearSubGraph<IStoryNode> SubGraph { get; set; } = null!;
    public bool IsAtEndOfSubGraph => SubGraph.NodeSequence.Count == Index;
    public bool IsInInvalidState => Index > SubGraph.NodeSequence.Count;

    internal IStoryNode GetIndexedNode()
    {
        return SubGraph.NodeSequence[Index];
    }
}
