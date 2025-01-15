using ScenarioModelling.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModelling.Collections.Graph;
using ScenarioModelling.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModelling.CodeHooks;

public class DefinitionScope
{
    private readonly Action _verifyPreviousDefinition;

    public List<INodeHookDefinition> NodeHookDefinitions { get; set; } = new();
    public SemiLinearSubGraph<IScenarioNode> SubGraph { get; set; } = null!;
    public int CurrentIndex { get; set; } = 0;

    public DefinitionScope(SemiLinearSubGraph<IScenarioNode> subGraph, Action verifyPreviousDefinition)
    {
        SubGraph = subGraph;
        _verifyPreviousDefinition = verifyPreviousDefinition;
    }

    public DefinitionScopeSnapshot TakeSnapshot()
    {
        return new DefinitionScopeSnapshot() { Index = CurrentIndex };
    }

    public void AddOrVerifyInPhase(INodeHookDefinition newNodeDefinition, Action add, Action<IScenarioNode> existing)
    {
        IScenarioNode newNode = newNodeDefinition.GetNode();

        if (newNodeDefinition.ScopeSnapshot.Index > SubGraph.NodeSequence.Count + 1)
            throw new Exception("Current index is too far beyond the end of the subgraph");

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
            var currentNode = SubGraph.NodeSequence[newNodeDefinition.ScopeSnapshot.Index];
            int indexAdvance = VerifyInPhaseWithGraph(currentNode, newNode);

            existing(currentNode);

            CurrentIndex += indexAdvance; // TODO This should be managed by the verify method because in looking ahead we will need to skip nodes
        }
    }

    private int VerifyInPhaseWithGraph(IScenarioNode existingNode, IScenarioNode newNode)
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
                    throw new Exception("Current index is too far beyond the end of the subgraph : To be implemented");

                IScenarioNode next = SubGraph.NodeSequence[CurrentIndex + 1];
                VerifyInPhaseWithGraph(next, newNode);
            }
            else
            {
                throw new Exception("Current node does not match the new definition");
            }
        }

        return 1;
    }

    internal void ReturnToStartOfScope()
    {
        _verifyPreviousDefinition();

        CurrentIndex = 0;
    }

}