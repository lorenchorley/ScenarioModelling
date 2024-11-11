using FluentAssertions;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Tests.Valid;
using System.Diagnostics;

namespace ScenarioModel.Tests;

[TestClass]
public class SerialisationTests
{
    [TestMethod]
    [TestCategory("Serialisation")]
    [SerialisationDataProvider]
    public void Deserialise_Serialise(string originalContext)
    {
        // Arrange
        // =======

        // Act
        // ===
        Debug.WriteLine("Starting Serialised Context");
        Debug.WriteLine("===========================");
        Debug.WriteLine("");
        Debug.WriteLine(originalContext);

        Context loadedContext =
            Context.New()
                   .UseSerialiser<HumanReadableSerialiser>()
                   .LoadContext(originalContext)
                   .Initialise();

        loadedContext.ValidationErrors.Count.Should().Be(0, because: loadedContext.ValidationErrors.ToString());

        loadedContext.Serialise()
                     .Switch(
            reserialisedContext =>
            {
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine("Reserialised Context");
                Debug.WriteLine("====================");
                Debug.WriteLine("");
                Debug.WriteLine(reserialisedContext);

                Context reloadedContext = Context.New()
                       .UseSerialiser<HumanReadableSerialiser>()
                       .LoadContext(reserialisedContext)
                       .Initialise();


                // Assert
                // ======
                reloadedContext.ValidationErrors.Count.Should().Be(0, $"because {string.Join('\n', reloadedContext.ValidationErrors)}");

                DiffAssert.DiffIfNotEqual(originalContext, reserialisedContext);
            },
            ex => Assert.Fail(ex.Message)
        );
    }

}
