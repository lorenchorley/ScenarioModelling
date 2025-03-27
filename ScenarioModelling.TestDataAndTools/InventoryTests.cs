using ScenarioModelling.TestDataAndTools.CodeHooks;
using ScenarioModelling.TestDataAndTools.Expressions;
using ScenarioModelling.TestDataAndTools.Serialisation;
using ScenarioModelling.TestDataAndTools.TestCases;
using System.Text;

namespace ScenarioModelling.TestDataAndTools;

[TestClass]
public class InventoryTests
{
    [TestMethod]
    [TestCategory("Inventory tests")]
    public async Task ExpressionGrammarTestDataProvider_Inventory()
    {
        // Arrange
        // =======


        // Act
        // ===
        var result = new ExpressionGrammarTestDataProviderAttribute().GetData(null).ToList();

        PrintObjectArray(result);


        // Assert
        // ======
        Assert.IsTrue(result.Any());


    }

    [TestMethod]
    [TestCategory("Inventory tests")]
    public async Task ReserialisationDataProvider_Inventory()
    {
        // Arrange
        // =======


        // Act
        // ===
        var result = new ReserialisationDataProviderAttribute().GetData(null).ToList();

        PrintObjectArray(result);


        // Assert
        // ======
        Assert.IsTrue(result.Any());


    }

    [TestMethod]
    [TestCategory("Inventory tests")]
    public async Task CustomSerialiserGrammarTestDataProvider_Inventory()
    {
        // Arrange
        // =======


        // Act
        // ===
        var result = new CustomSerialiserGrammarTestDataProviderAttribute().GetData(null).ToList();

        PrintObjectArray(result);


        // Assert
        // ======
        Assert.IsTrue(result.Any());


    }

    [TestMethod]
    [TestCategory("Inventory tests")]
    public async Task ProgressiveCodeHookTestDataProvider_Inventory()
    {
        // Arrange
        // =======


        // Act
        // ===
        var result = new ProgressiveCodeHookTestDataProviderAttribute().GetData(null).ToList();

        PrintObjectArray(result);


        // Assert
        // ======
        Assert.IsTrue(result.Any());


    }

    [TestMethod]
    [TestCategory("Inventory tests")]
    public async Task TestCaseTestDataProvider_Inventory()
    {
        // Arrange
        // =======


        // Act
        // ===
        var result = new TestCaseTestDataProviderAttribute().GetData(null).ToList();

        PrintObjectArray(result);


        // Assert
        // ======
        Assert.IsTrue(result.Any());

    }

    private static void PrintObjectArray(List<object[]> result)
    {
        StringBuilder sb = new();

        foreach (var item in result)
        {
            for (int i = 0; i < item.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append($"{item[i]} ({item[i].GetType().Name})");
            }
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }
}
