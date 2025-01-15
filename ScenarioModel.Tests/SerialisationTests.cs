using FluentAssertions;
using ScenarioModelling.Serialisation.HumanReadable.Reserialisation;
using ScenarioModelling.Tests.Valid;
using System.Diagnostics;

namespace ScenarioModelling.Tests;

[TestClass]
public class SerialisationTests
{
    [TestMethod]
    [TestCategory("Serialisation")]
    [ReserialisationDataProvider]
    public void Context_DeserialiseReserialise(string testCaseName, string originalContextText, string expectedFinalContextText)
    {
        // Arrange
        // =======

        // Act
        // ===
        Debug.WriteLine("Starting Serialised Context");
        Debug.WriteLine("===========================");
        Debug.WriteLine("");
        Debug.WriteLine(originalContextText);

        Context loadedContext =
            Context.New()
                   .UseSerialiser<ContextSerialiser>()
                   .LoadContext(originalContextText)
                   .Initialise();

        loadedContext.ValidationErrors.Count.Should().Be(0, because: loadedContext.ValidationErrors.ToString());

        loadedContext.Serialise()
                     .Switch(
            reserialisedContextText =>
            {
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine("Reserialised Context");
                Debug.WriteLine("====================");
                Debug.WriteLine("");
                Debug.WriteLine(reserialisedContextText);

                Context reloadedContext = Context.New()
                       .UseSerialiser<ContextSerialiser>()
                       .LoadContext(reserialisedContextText)
                       .Initialise();


                // Assert
                // ======
                reloadedContext.ValidationErrors.Count.Should().Be(0, $"because {string.Join('\n', reloadedContext.ValidationErrors)}");

                DiffAssert.DiffIfNotEqual(expectedFinalContextText.Trim(), reserialisedContextText.Trim(), leftName: $@"Expected_{testCaseName.Replace(' ', '_')}", rightName: $@"Result_{testCaseName.Replace(' ', '_')}");
            },
            ex => Assert.Fail(ex.Message)
        );
    }

}
