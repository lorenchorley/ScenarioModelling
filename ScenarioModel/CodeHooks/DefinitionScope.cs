using ScenarioModel.CodeHooks.HookDefinitions.Interfaces;
using ScenarioModel.Collections.Graph;
using ScenarioModel.Objects.ScenarioNodes.BaseClasses;

namespace ScenarioModel.CodeHooks;

public class DefinitionScope
{
    public List<INodeHookDefinition> NodeHookDefinitions { get; set; } = new();
    public SemiLinearSubGraph<IScenarioNode> SubGraph { get; set; } = null!;
    public int CurrentIndex { get; set; } = 0;

    public void AddOrVerifyInPhase(INodeHookDefinition nodeDefinition, Action add)
    {
        IScenarioNode newNode = nodeDefinition.GetNode();

        if (CurrentIndex > SubGraph.NodeSequence.Count + 1)
            throw new Exception("Current index is too far beyond the end of the subgraph");

        bool atEndOfSubgraph = SubGraph.NodeSequence.Count == CurrentIndex;

        // If the index points to just after the last position, then we must be missing the next mode. We add it
        if (atEndOfSubgraph)
        {
            NodeHookDefinitions.Add(nodeDefinition);
            SubGraph.NodeSequence.Add(newNode);

            add();
        }
        else // Otherwise we check that the current node corresponds to the new definition
        {
            var currentNode = SubGraph.NodeSequence[CurrentIndex];
            bool nodesAreEssentiallyTheSame = currentNode.IsFullyEqv(newNode);
            if (!nodesAreEssentiallyTheSame)
                throw new Exception("Current node does not match the new definition");
        }

        CurrentIndex++;
    }

    internal void ReturnToStartOfScope()
    {
        CurrentIndex = 0;
    }

    //internal void SetCurrentNodeDefintion(INodeHookDefinition nodeDefinition)
    //{
    //    var node = nodeDefinition.GetNode();
    //    int? index = SubGraph.NodeSequence.IndexOf(node);

    //    if (index is null)
    //    {
    //        throw new Exception("Node not found in subgraph");
    //    }

    //    CurrentIndex = (int)index;
    //}
}