using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
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
    public SemiLinearSubGraph<IStoryNode> SubGraph { get; set; } = null!;
    public int CurrentIndex { get; set; } = 0;

    public SubgraphScopedHookSynchroniser(SemiLinearSubGraph<IStoryNode> subGraph, Action verifyPreviousDefinition)
    {
        SubGraph = subGraph;
        _verifyPreviousDefinition = verifyPreviousDefinition;
    }

    internal DefinitionScopeSnapshot TakeSnapshot()
    {
        return new DefinitionScopeSnapshot()
        {
            Scope = this,
            Index = CurrentIndex
        };
    }

    internal void AddOrVerifyInPhase(INodeHookDefinition newNodeDefinition, Action add, Action<IStoryNode> existing)
    {
        IStoryNode newNode = newNodeDefinition.GetNode();

        bool atEndOfSubgraph = SubGraph.NodeSequence.Count == newNodeDefinition.ScopeSnapshot.Index;

        // If the index points to just after the last position, then we must be missing the next mode. We add it
        if (atEndOfSubgraph)
        {
            NodeHookDefinitions.Add(newNodeDefinition);
            SubGraph.NodeSequence.Add(newNode);

            add();
            CurrentIndex++;
        }
        else // Otherwise we check that the current node corresponds to the new definition
        {
            if (newNodeDefinition.ScopeSnapshot.Scope != this)
                throw new InternalLogicException("Scope snapshot does not correspond to this scope, this indicates something wrong with the graph flow algorithm");

            if (newNodeDefinition.ScopeSnapshot.Index > SubGraph.NodeSequence.Count)
                throw new InternalLogicException("Current index is too far beyond the end of the subgraph, this indicates something wrong with the graph flow algorithm");

            var currentNode = SubGraph.NodeSequence[newNodeDefinition.ScopeSnapshot.Index];
            int indexAdvance = VerifyInPhaseWithGraph(currentNode, newNode);

            existing(currentNode);

            CurrentIndex += indexAdvance; // TODO This should be managed by the verify method because in looking ahead we will need to skip nodes
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

                if (CurrentIndex + 1 >= SubGraph.NodeSequence.Count)
                    throw new NotImplementedException("Current index is too far beyond the end of the subgraph : To be implemented");

                IStoryNode next = SubGraph.NodeSequence[CurrentIndex + 1];
                return VerifyInPhaseWithGraph(next, newNode); // return ?
            }

            throw new InternalLogicException($"Current node does not match the new definition\nCurrent node : {existingNode}\nNew definition : {newNode}");
        }

        return 1;
    }

    internal void ReturnToStartOfScope()
    {
        _verifyPreviousDefinition();

        CurrentIndex = 0;
    }
}