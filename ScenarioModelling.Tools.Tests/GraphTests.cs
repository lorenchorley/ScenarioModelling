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
        DirectedGraph<Node> graph = new();
        graph.Add(new("1"));
        graph.Add(new("2"));
        graph.Add(new("3"));

        SemiLinearSubGraph<Node> subgraph = new SemiLinearSubGraph<Node>();
        subgraph.NodeSequence.Add(new("4.1"));

        SemiLinearSubGraph<Node> subsubgraph = new SemiLinearSubGraph<Node>();
        subsubgraph.NodeSequence.Add(new("4.2.1"));
        subsubgraph.NodeSequence.Add(new("4.2.2"));
        subgraph.NodeSequence.Add(new NodeWithSubgraph("4.2", subsubgraph));

        subgraph.NodeSequence.Add(new("4.3"));
        graph.Add(new NodeWithSubgraph("4", subgraph));

        graph.Add(new("5"));
        graph.Add(new("6"));

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
            var subgraphScope = graph.PrimarySubGraph.GenerateScope(null);
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
            SemiLinearSubGraphScope<Node> subgraph = graph.PrimarySubGraph.GenerateScope(null);
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
                    subgraph = newSubgraphNode.Subgraph.GenerateScope(subgraph);
                }

                // Get the next node in the current subgraph
                node = subgraph.MoveToNextInSequence();

                // If we have reached the end of the subgraph, we must return to the parent subgraph
                // unless there is none, in which case the traversal is finished
                if (node == null && subgraph.ParentScope != null)
                {
                    subgraph = subgraph.ParentScope;
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
