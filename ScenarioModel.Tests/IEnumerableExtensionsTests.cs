using FluentAssertions;

namespace ScenarioModel.Tests;

[TestClass]
public class IEnumerableExtensionsTests
{

    [TestMethod]
    [TestCategory("IEnumerable Extensions")]
    public void AllStates_ReturnsAllStates()
    {
        // Arrange
        // =======
        object obj1 = new object();
        object obj2 = new object();
        object obj3 = new object();

        List<object> allUnique = new()
        {
            obj1,
            obj2,
            obj3
        };

        List<object> allSame = new()
        {
            obj1,
            obj1,
            obj1,
            obj1
        };

        List<object> mixed = new()
        {
            obj1,
            obj1,
            obj1,
            obj2,
            obj2,
            obj3
        };

        List<object> someMixed = new()
        {
            obj1,
            obj1,
            obj1,
            obj2
        };


        // Act
        // ===
        int allUniqueCount = allUnique.UniqueObjectInstanceCount();
        int allSameCount = allSame.UniqueObjectInstanceCount();
        int mixedCount = mixed.UniqueObjectInstanceCount();
        int someMixedCount = someMixed.UniqueObjectInstanceCount();


        // Assert
        // ======
        allUniqueCount.Should().Be(3);
        allSameCount.Should().Be(1);
        mixedCount.Should().Be(3);
        someMixedCount.Should().Be(2);

    }
}