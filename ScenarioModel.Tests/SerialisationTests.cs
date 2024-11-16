using FluentAssertions;
using LanguageExt.Common;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Tests.Valid;
using System.Diagnostics;

namespace ScenarioModel.Tests;

[TestClass]
public class SerialisationTests
{
    [TestMethod]
    [TestCategory("Serialisation")]
    [ReserialisationDataProvider]
    public void Deserialise_Serialise(string testCaseName, string originalContextText, string expectedFinalContextText)
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
                   .UseSerialiser<HumanReadableSerialiser>()
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
                       .UseSerialiser<HumanReadableSerialiser>()
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
