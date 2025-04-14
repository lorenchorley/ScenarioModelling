namespace WebDesigner.Blazor.Client;

public static class TestData
{
    public static string StartingText =
    """
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

    MetaStory "MetaStory recorded by hooks" {
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
    """;

}
