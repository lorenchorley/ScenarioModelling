using FluentAssertions;
using LanguageExt.Common;
using ScenarioModel.Serialisation;
using ScenarioModel.Serialisation.HumanReadable.Reserialisation;
using ScenarioModel.Tests.Valid;
using System.Diagnostics;

namespace ScenarioModel.Tests;

[TestClass]
public class SerialisationTests
{
    [TestMethod]
    [TestCategory("Serialisation")]
    public void Deserialise_Serialise_AllContexts()
    {
        // Arrange
        // =======
        List<string> contexts =
        [
            ValidScenario1.SerialisedContext
        ];

        TestDeserialiseSerialiseContextForSerialiser<HumanReadableSerialiser>(contexts);
        //TestSerialiseDeserialiseSystemsForSerialiser<YamlSerialiserV1>(systems);
    }

    private static void TestDeserialiseSerialiseContextForSerialiser<T>(List<string> contexts) where T : ISerialiser, new()
    {
        foreach (string context in contexts)
        {
            // Act
            // ===
            Debug.WriteLine("Starting Serialised Context");
            Debug.WriteLine("===========================");
            Debug.WriteLine("");
            Debug.WriteLine(context);

            Context loadedContext =
                Context.New()
                       .UseSerialiser<T>()
                       .LoadContext<T>(context)
                       .Initialise();

            loadedContext.ValidationErrors.Count.Should().Be(0, because: loadedContext.ValidationErrors.ToString());

            Result<string> reserialisedContext = loadedContext.Serialise<T>();

            reserialisedContext.IfFail(ex => Assert.Fail(ex.Message));

            reserialisedContext.IfSucc(newContext =>
            {
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine("Reserialised Context");
                Debug.WriteLine("====================");
                Debug.WriteLine("");
                Debug.WriteLine(newContext);

                Context reloadedContext = Context.New()
                       .UseSerialiser<T>()
                       .LoadContext<T>(newContext)
                       .Initialise();


                // Assert
                // ======
                reloadedContext.ValidationErrors.Count.Should().Be(0, $"because {string.Join('\n', reloadedContext.ValidationErrors)}");
                newContext.Trim().Should().BeEquivalentTo(context.Trim());
            });
        }
    }

}
