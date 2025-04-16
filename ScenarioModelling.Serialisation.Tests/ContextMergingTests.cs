using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;
using ScenarioModelling.TestDataAndTools;

namespace ScenarioModelling.Serialisation.Tests;

[TestClass]
public class ContextMergingTests
{
    private const string _firstText = """
        Entity Actor {
            State "Amy Stake"
            CharacterStyle Red
        }
        
        StateMachine Name {
            State "Amy Stake"
            State "Brock Lee"
            State "Clara Nett"
            State "Dee Zaster"
            "Amy Stake" -> "Brock Lee" : ChangeName
            "Brock Lee" -> "Clara Nett" : ChangeName
            "Clara Nett" -> "Dee Zaster" : ChangeName
        }
        
        MetaStory "MainMetaStory" {
            Dialog SayName {
                Text "The actor {Actor.State} says hello and introduces themselves"
                Character Actor
            }
            While <Actor.State != "Dee Zaster"> {
                If <Actor.State == "Amy Stake"> {
                }
                If <Actor.State == "Brock Lee"> {
                    Dialog {
                        Text "The actor Mr Lee makes a bad pun to do with their name"
                        Character Actor
                    }
                }
                If <Actor.State == "Clara Nett"> {
                    Dialog {
                        Text "The actor Mrs Nett makes a bad pun to do with their name"
                        Character Actor
                    }
                }
                Transition {
                    Actor : ChangeName
                }
            }
            If <Actor.State == "Dee Zaster"> {
                Dialog {
                    Text "The actor Mr Zaster makes a bad pun to do with their name"
                    Character Actor
                }
            }
        }
        """;

    private const string _secondText = """
        Entity Actor {
            State "Amy Stake"
            CharacterStyle Red
        }
        
        Entity ExtraEntity {
            State "ExtraState"
        }

        StateMachine ExtraStateMachine {
            State "ExtraState"
            State "ExtraState2"
            "ExtraState" -> "ExtraState2" : ChangeName
        }

        MetaStory "MainMetaStory" {
            Dialog SayName {
                Text "The actor {Actor.State} says hello and introduces themselves"
                Character Actor
            }
            While <Actor.State != "Dee Zaster"> {
                If <Actor.State == "Amy Stake"> {
                    Dialog {
                        Text "The actor Mrs Stake makes a bad pun to do with their name"
                        Character Actor
                    }
                }
                If <Actor.State == "Brock Lee"> {
                }
                If <Actor.State == "Clara Nett"> {
                }
                Transition {
                    Actor : ChangeName
                }
            }
            If <Actor.State == "Dee Zaster"> {
            }
        }

        MetaStory "Extra MetaStory" {
            Dialog SayName {
                Text "Some dialog"
                Character ExtraEntity
            }
        }
        """;

    private const string _expectedFinalText = """
        Entity Actor {
          State "Amy Stake"
          CharacterStyle Red
        }
        
        Entity ExtraEntity {
          State ExtraState
        }

        StateMachine Name {
          State "Amy Stake"
          State "Brock Lee"
          State "Clara Nett"
          State "Dee Zaster"
          "Amy Stake" -> "Brock Lee" : ChangeName
          "Brock Lee" -> "Clara Nett" : ChangeName
          "Clara Nett" -> "Dee Zaster" : ChangeName
        }
        
        StateMachine ExtraStateMachine {
          State ExtraState
          State ExtraState2
          ExtraState -> ExtraState2 : ChangeName
        }
        
        MetaStory MainMetaStory {
          Dialog SayName {
            Text "The actor {Actor.State} says hello and introduces themselves"
            Character Actor
          }
          While <Actor.State != "Dee Zaster"> {
            If <Actor.State == "Amy Stake"> {
              Dialog {
                Text "The actor Mrs Stake makes a bad pun to do with their name"
                Character Actor
              }
            }
            If <Actor.State == "Brock Lee"> {
              Dialog {
                Text "The actor Mr Lee makes a bad pun to do with their name"
                Character Actor
              }
            }
            If <Actor.State == "Clara Nett"> {
              Dialog {
                Text "The actor Mrs Nett makes a bad pun to do with their name"
                Character Actor
              }
            }
            Transition {
              Actor : ChangeName
            }
          }
          If <Actor.State == "Dee Zaster"> {
            Dialog {
              Text "The actor Mr Zaster makes a bad pun to do with their name"
              Character Actor
            }
          }
        }

        MetaStory "Extra MetaStory" {
          Dialog SayName {
            Text "Some dialog"
            Character ExtraEntity
          }
        }
        """;

    [TestMethod]
    [TestCategory("ContextMerging")]
    public void ContextMergingTest()
    {
        // Arrange
        // =======
        using ScenarioModellingContainer container = new();
        using var scope = container.StartScope();


        // Act
        // ===
        scope.Context
             .UseSerialiser<CustomContextSerialiser>()
             .LoadContext(_firstText)
             .LoadContext(_secondText)
             .Initialise();


        // Assert
        // ======
        var serialisedContext =
            scope.Context
                 .ResetToInitialState()
                 .Serialise()
                 .Match(v => v, e => throw e)
                 .Trim();

        DiffAssert.DiffIfNotEqual(serialisedContext.Trim(), _expectedFinalText.Trim());

    }
}
