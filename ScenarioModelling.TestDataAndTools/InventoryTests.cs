using ScenarioModelling.TestDataAndTools.CodeHooks;
using ScenarioModelling.TestDataAndTools.Expressions;
using ScenarioModelling.TestDataAndTools.Serialisation;

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
        var result = new ExpressionGrammarTestDataProviderAttribute().GetData(null);


        // Assert
        // ======


    }

    [TestMethod]
    [TestCategory("Inventory tests")]
    public async Task ReserialisationDataProvider_Inventory()
    {
        // Arrange
        // =======


        // Act
        // ===
        var result = new ReserialisationDataProviderAttribute().GetData(null);


        // Assert
        // ======


    }

    [TestMethod]
    [TestCategory("Inventory tests")]
    public async Task HumanReadableGrammarTestDataProvider_Inventory()
    {
        // Arrange
        // =======


        // Act
        // ===
        var result = new HumanReadableGrammarTestDataProviderAttribute().GetData(null);


        // Assert
        // ======


    }

    [TestMethod]
    [TestCategory("Inventory tests")]
    public async Task ProgressiveCodeHookTestDataProvider_Inventory()
    {
        // Arrange
        // =======


        // Act
        // ===
        var result = new ProgressiveCodeHookTestDataProviderAttribute().GetData(null);


        // Assert
        // ======


    }
}
