using FluentAssertions;
using ScenarioModelling.Exhaustiveness.Attributes;
using ScenarioModelling.Exhaustiveness.Common;
using ScenarioModelling.Exhaustiveness.Exceptions;

namespace ScenarioModelling.Tests;

#region Target classes
public class First { }
public class Second { }
public class Third { }
#endregion

#region Fully implemented
internal interface IFullyImplementedTestNodeBase { }

[StoryNodeLike<IFullyImplementedTestNodeBase, First>]
internal class FirstFullyImplemented : IFullyImplementedTestNodeBase { }

[StoryNodeLike<IFullyImplementedTestNodeBase, Second>]
internal class SecondFullyImplemented : IFullyImplementedTestNodeBase { }

[StoryNodeLike<IFullyImplementedTestNodeBase, Third>]
internal class ThirdFullyImplemented : IFullyImplementedTestNodeBase { }
#endregion

#region Not fully implemented
internal interface INotFullyImplementedTestNodeBase { }

[StoryNodeLike<INotFullyImplementedTestNodeBase, First>]
internal class FirstNotFullyImplemented : INotFullyImplementedTestNodeBase { }

[StoryNodeLike<INotFullyImplementedTestNodeBase, Second>]
internal class SecondNotFullyImplemented : INotFullyImplementedTestNodeBase { }

//[NodeLike<INotFullyImplementedTestNodeBase, Third>]
//internal class ThirdNotFullyImplemented : INotFullyImplementedTestNodeBase { }
#endregion

#region Doubly implemented
internal interface IDoublyImplementedTestNodeBase { }

[StoryNodeLike<IDoublyImplementedTestNodeBase, First>]
internal class FirstDoublyImplemented : IDoublyImplementedTestNodeBase { }

[StoryNodeLike<IDoublyImplementedTestNodeBase, Second>]
internal class SecondDoublyImplemented : IDoublyImplementedTestNodeBase { }

[StoryNodeLike<IDoublyImplementedTestNodeBase, Third>]
internal class FirstThirdDoublyImplemented : IDoublyImplementedTestNodeBase { }

// Secon class that is annotated with the same target class (Third)
[StoryNodeLike<IDoublyImplementedTestNodeBase, Third>]
internal class SecondThirdDoublyImplemented : IDoublyImplementedTestNodeBase { }
#endregion

#region Overly implemented
public class Fourth { } // Shall not be present in the complete list of types

internal interface IOverlyImplementedTestNodeBase { }

[StoryNodeLike<IOverlyImplementedTestNodeBase, First>]
internal class FirstOverlyImplemented : IOverlyImplementedTestNodeBase { }

[StoryNodeLike<IOverlyImplementedTestNodeBase, Second>]
internal class SecondOverlyImplemented : IOverlyImplementedTestNodeBase { }

[StoryNodeLike<IOverlyImplementedTestNodeBase, Third>]
internal class ThirdOverlyImplemented : IOverlyImplementedTestNodeBase { }

[StoryNodeLike<IOverlyImplementedTestNodeBase, Fourth>]
internal class FourthOverlyImplemented : IOverlyImplementedTestNodeBase { }
#endregion

[TestClass]
public class ExhaustivenssTests
{
    private Type[] _completeTypeList = [typeof(First), typeof(Second), typeof(Third)];

    [AssemblyInitialize()]
    public static void AssemblyInit(TestContext context)
    {
        ExhaustivityFunctions.Active = true;
    }

    [TestMethod]
    [TestCategory("Exhaustivenss")]
    public void FullyImplemented()
    {
        // Arrange
        // =======
        TypeExhaustivityFunctions functions = new(_completeTypeList, typeof(StoryNodeLikeAttribute<,>), "test");


        // Act && Assert
        // =============
        functions.AssertTypeExhaustivelyImplemented<IFullyImplementedTestNodeBase>();

    }

    [TestMethod]
    [TestCategory("Exhaustivenss")]
    public void NotFullyImplemented()
    {
        // Arrange
        // =======
        TypeExhaustivityFunctions functions = new(_completeTypeList, typeof(StoryNodeLikeAttribute<,>), "test");


        // Act
        // ===
        Action action = () => functions.AssertTypeExhaustivelyImplemented<INotFullyImplementedTestNodeBase>();


        // Assert
        // ======
        action.Should().Throw<ExhaustivenessException>()
              .Which.Types.Should().BeEquivalentTo([typeof(Third)]);

    }

    [TestMethod]
    [TestCategory("Exhaustivenss")]
    public void DoublyImplemented_ExtraImplementation()
    {
        // Arrange
        // =======
        TypeExhaustivityFunctions functions = new(_completeTypeList, typeof(StoryNodeLikeAttribute<,>), "test");


        // Act
        // ===
        Action action = () => functions.AssertTypeExhaustivelyImplemented<IDoublyImplementedTestNodeBase>();


        // Assert
        // ======
        action.Should().Throw<ExhaustivenessException>()
              .Which.Types.Should().BeEquivalentTo([typeof(FirstThirdDoublyImplemented), typeof(SecondThirdDoublyImplemented)]);

    }

    [TestMethod]
    [TestCategory("Exhaustivenss")]
    public void OverlyImplemented_UntaggedImplementation()
    {
        // Arrange
        // =======
        TypeExhaustivityFunctions functions = new(_completeTypeList, typeof(StoryNodeLikeAttribute<,>), "test");


        // Act
        // ===

        // The class "Fourth" is not in the complete list, but it has been tagged in an attribute (on SecondThirdOverlyImplemented2)
        Action action = () => functions.AssertTypeExhaustivelyImplemented<IOverlyImplementedTestNodeBase>();


        // Assert
        // ======
        action.Should().Throw<ExhaustivenessException>()
              .Which.Types.Should().BeEquivalentTo([typeof(Fourth)]);

    }
}