using ScenarioModelling.CoreObjects.MetaStoryNodes.BaseClasses;
using ScenarioModelling.Tools.Collections.Graph;
using System.Diagnostics;

namespace ScenarioModelling.Tests;

[TestClass]
[UsesVerify]
public partial class GraphTests
{
    [DebuggerDisplay("Node : {Name}")]
    private class Node(string name) : IDirectedGraphNode<Node>
    {
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Type Type => throw new NotImplementedException();

        public IEnumerable<SemiLinearSubGraph<Node>> TargetSubgraphs()
        {
            throw new NotImplementedException();
        }
    }

    private class NodeWithSubgraph(string name, SemiLinearSubGraph<Node> subgraph) : Node(name)
    {
        public SemiLinearSubGraph<Node> Subgraph => subgraph;
    }

    private static DirectedGraph<Node> SetupGraph()
    {
        var primarySubGraph = new SemiLinearSubGraph<Node>();
        DirectedGraph<Node> graph = new(primarySubGraph);
        primarySubGraph.AddToSequence(new("1"));
        primarySubGraph.AddToSequence(new("2"));
        primarySubGraph.AddToSequence(new("3"));

        SemiLinearSubGraph<Node> subgraph = new SemiLinearSubGraph<Node>();
        subgraph.AddToSequence(new("4.1"));

        SemiLinearSubGraph<Node> subsubgraph = new SemiLinearSubGraph<Node>();
        subsubgraph.AddToSequence(new("4.2.1"));
        subsubgraph.AddToSequence(new("4.2.2"));
        subgraph.AddToSequence(new NodeWithSubgraph("4.2", subsubgraph));

        subgraph.AddToSequence(new("4.3"));
        primarySubGraph.AddToSequence(new NodeWithSubgraph("4", subgraph));

        primarySubGraph.AddToSequence(new("5"));
        primarySubGraph.AddToSequence(new("6"));

        return graph;
    }

    [TestMethod]
    [TestCategory("Graphs")]
    public void TraverseGraphWithSubgraph_TopLevelTraversal()
    {
        // Arrange
        // =======
        var graph = SetupGraph();

        IEnumerable<Node> GenerateSequence()
        {
            var subgraphScope = (SemiLinearSubGraphScope<Node>)graph.PrimarySubGraph.GenerateScope(null);
            Node? node = subgraphScope.MoveToNextInSequence();
            while (node != null)
            {
                yield return node;
                node = subgraphScope.MoveToNextInSequence();
            }
        }

        // Act
        // ===
        var sequenceOfIdentifiers =
            GenerateSequence()
                .Select(n => n.Name)
                .ToList();


        // Assert
        // ======
        Assert.AreEqual("1, 2, 3, 4, 5, 6", string.Join(", ", sequenceOfIdentifiers));

    }

    [TestMethod]
    [TestCategory("Graphs")]
    public void TraverseGraphWithSubgraph_SubgraphTraversal_WithExplicitReentryPoint()
    {
        // Arrange
        // =======
        var graph = SetupGraph();

        IEnumerable<Node> GenerateSequence()
        {
            var subgraph = (SemiLinearSubGraphScope<Node>)graph.PrimarySubGraph.GenerateScope(null);
            Node? node = subgraph.MoveToNextInSequence();

            while (node != null)
            {
                yield return node;

                // If this node contains a subgraph, we can enter it (In this test behaviour, we do so everytime)
                if (node is NodeWithSubgraph newSubgraphNode)
                {
                    // Before we leave the current subgraph, we must record where we should come back to
                    //subgraph.SetExplicitNextNode(node);

                    // Enter the subgraph
                    //newSubgraphNode.Subgraph.ParentSubgraph = subgraph;
                    subgraph = (SemiLinearSubGraphScope<Node>)newSubgraphNode.Subgraph.GenerateScope(subgraph);
                }

                // Get the next node in the current subgraph
                node = subgraph.MoveToNextInSequence();

                // If we have reached the end of the subgraph, we must return to the parent subgraph
                // unless there is none, in which case the traversal is finished
                if (node == null && subgraph.ParentScope != null)
                {
                    subgraph = (SemiLinearSubGraphScope<Node>)subgraph.ParentScope;
                    node = subgraph.MoveToNextInSequence();
                }
            }
        }


        // Act
        // ===
        var sequenceOfIdentifiers =
            GenerateSequence()
                .Select(n => n.Name)
                .ToList();


        // Assert
        // ======
        Assert.AreEqual("1, 2, 3, 4, 4.1, 4.2, 4.2.1, 4.2.2, 4.3, 5, 6", string.Join(", ", sequenceOfIdentifiers));

    }

}
