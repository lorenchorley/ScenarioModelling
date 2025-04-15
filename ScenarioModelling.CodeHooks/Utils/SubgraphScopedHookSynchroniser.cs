using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes.Interfaces;
using ScenarioModelling.Tools.Collections.Graph;
using ScenarioModelling.Tools.Exceptions;

namespace ScenarioModelling.CodeHooks.Utils;

/// <summary>
/// 
/// </summary>
public class SubgraphScopedHookSynchroniser
{
    private readonly Action _verifyPreviousDefinition;

    public List<INodeHookDefinition> NodeHookDefinitions { get; set; } = new();
    public SemiLinearSubGraph<IStoryNode> SubGraph { get; set; } = null!; // This class is designed for semi linear subgraphs
    //public int CurrentIndex { get; set; } = 0;
    public SubGraphScopeSnapshot SubGraphTracker { get; set; } = new();

    public SubgraphScopedHookSynchroniser(SemiLinearSubGraph<IStoryNode> subGraph, Action verifyPreviousDefinition)
    {
        SubGraph = subGraph;
        _verifyPreviousDefinition = verifyPreviousDefinition;

        SubGraphTracker.Index = 0;
        SubGraphTracker.SubGraph = subGraph;
    }

    internal SubGraphScopeSnapshot TakeSnapshot()
    {
        return new SubGraphScopeSnapshot() // TODO Make this a function of the subgraph scope
        {
            SubGraph = SubGraph,
            Index = SubGraphTracker.Index
        };
    }

    internal void AddOrVerifyInPhase(INodeHookDefinition newNodeDefinition, Action add, Action<IStoryNode> existing)
    {
        IStoryNode newNode = newNodeDefinition.GetNode();

        //bool atEndOfSubgraph = SubGraph.NodeSequence.Count == newNodeDefinition.ScopeSnapshot.Index;
        bool atEndOfSubgraph = newNodeDefinition.ScopeSnapshot.IsAtEndOfSubGraph;

        // If the index points to just after the last position, then we must be missing the next mode. We add it
        if (atEndOfSubgraph)
        {
            NodeHookDefinitions.Add(newNodeDefinition);
            SubGraph.AddToSequence(newNode);

            add();
            SubGraphTracker.Index++;
        }
        else // Otherwise we check that the current node corresponds to the new definition
        {
            if (newNodeDefinition.ScopeSnapshot.SubGraph != SubGraph)
                throw new InternalLogicException("Scope snapshot does not correspond to this scope, this indicates something wrong with the graph flow algorithm");

            //if (newNodeDefinition.ScopeSnapshot.Index > SubGraph.NodeSequence.Count)
            if (newNodeDefinition.ScopeSnapshot.IsInInvalidState)
                throw new InternalLogicException("Current index is too far beyond the end of the subgraph, this indicates something wrong with the graph flow algorithm");

            //var currentNode = SubGraph.NodeSequence[newNodeDefinition.ScopeSnapshot.Index];
            var currentNode = newNodeDefinition.ScopeSnapshot.GetIndexedNode();
            int indexAdvance = VerifyInPhaseWithGraph(currentNode, newNode);

            existing(currentNode);

            SubGraphTracker.Index += indexAdvance; // TODO This should be managed by the verify method because in looking ahead we will need to skip nodes
        }
    }

    private int VerifyInPhaseWithGraph(IStoryNode existingNode, IStoryNode newNode)
    {
        bool nodesAreEssentiallyTheSame = existingNode.IsFullyEqv(newNode);
        if (!nodesAreEssentiallyTheSame)
        {
            if (existingNode.Implicit)
            {
                // If the current node doesn't match the new definition, and the current node is implicit
                // we need to look ahead until we find a non implicit node that either matches or not the new definition

                // Simple implementation for now : no look ahead outside the current subgraph
                // TODO arbitrary look ahead, up and down all possible subgraphs from the current node

                if (SubGraphTracker.Index + 1 >= SubGraph.NodeSequence.Count) // This requires a semi linear graph, but this class is designed for that
                    throw new NotImplementedException("Current index is too far beyond the end of the subgraph : To be implemented");

                IStoryNode next = SubGraph.NodeSequence[SubGraphTracker.Index + 1]; // This requires a semi linear graph, but this class is designed for that
                return VerifyInPhaseWithGraph(next, newNode); // return ?
            }

            throw new InternalLogicException($"Current node does not match the new definition\nCurrent node : {existingNode}\nNew definition : {newNode}");
        }

        return 1;
    }

    internal void ReturnToStartOfScope()
    {
        _verifyPreviousDefinition();

        SubGraphTracker.Index = 0;
    }
}