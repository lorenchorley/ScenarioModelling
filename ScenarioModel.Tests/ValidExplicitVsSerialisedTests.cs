//using FluentAssertions;
//using MetaStoryModel.Execution;
//using MetaStoryModel.Serialisation.HumanReadable.Reserialisation;
//using MetaStoryModel.Tests.Valid;

//namespace MetaStoryModel.Tests;

//[TestClass]
//public class ValidExplicitVsSerialisedTests
//{
//    [TestMethod]
//    [TestCategory("Valid"), TestCategory("Serialisation")]
//    public void MetaStory1_ExplicitAndSerialised_ValidatesRunsEquals()
//    {
//        // Arrange
//        // =======
//        var explicitContext =
//            Context.New()
//                   .UseSerialiser<HumanReadableSerialiser>()
//                   .LoadSystem(ValidMetaStory1.System, out System system)
//                   .LoadMetaStory(ValidMetaStory1.MetaStory, out MetaStory MetaStory)
//                   .Initialise();

//        explicitContext.ValidationErrors.Should().BeEmpty();

//        var fromSerialisedContext =
//            Context.New()
//                   .UseSerialiser<HumanReadableSerialiser>()
//                   .LoadContext<HumanReadableSerialiser>(ValidMetaStory1.SerialisedContext)
//                   .Initialise();

//        fromSerialisedContext.ValidationErrors.Should().BeEmpty();


//        // Act
//        // ===
//        StoryRunResult resultFromExplicit =
//            explicitContext.MetaStories
//                           .Where(s => s.Name == nameof(ValidMetaStory1))
//                           .First()
//                           .StartAtStep("S1");

//        StoryRunResult resultFromSerialised =
//            fromSerialisedContext.MetaStories
//                           .Where(s => s.Name == nameof(ValidMetaStory1))
//                           .First()
//                           .StartAtStep("S1");


//        // Assert
//        // ======

//        var explicitContextSerialised = explicitContext.Serialise<HumanReadableSerialiser>();
//        var fromSerialisedContextReserialised = fromSerialisedContext.Serialise<HumanReadableSerialiser>();
//        explicitContextSerialised.Should().Be(fromSerialisedContextReserialised);

//        // Check final state of system
//        resultFromExplicit.Should().BeOfType<Successful>().Which.Story.Events.Should().HaveCount(1);
//        fromSerialisedContext.Should().BeOfType<Successful>().Which.Story.Events.Should().HaveCount(1);
//    }
//}