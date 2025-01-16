# Warning

This project is still in its experimental stage.

# TL;DR

This tool provides a way to build a strong verifiable link between functional requirements and final implmentation.

It does so through the use of an intermediate artifact, which has the name Meta Story that follows from the agile term User Story.

A Meta Story acts as a bridge between all stages of the software development lifecycle, from conception to maintenance, and contains the essentiel logic required to encapsulate the functional requirements of a product.

This intermediate artifact is humanly comprehensible like a UML diagram and can be used to automatically or manually analyse and test a product, and even improve it through bug analysis tooling.


# Introduction and motivation

This section is still under construction, here are all the essential ideas :

* Lack of strong tooling that goes from the beginning of the process at client requirement gathering to the end at maintenance, that is accessible and useful for every role in the process.
* Compliments the standard documentation, development and testing.
* Provides a strong, automatically verifiable, link between requirements and code.
    * Enables a developer to immdeiately locate the code that responds to a given functional requirement and vise versa.
* Improved exploration of the possibility space in client requirements gathering, debugging and testing.
* Interactive, comprehensible, analysable.
* Exportable into standard artefacts like UML diagrams.
* Can be put in place after development to analyse, comprehend and strengthen a legacy project.

* It is a tool that allows for the creation of meta stories that can be run and analysed. 
* The meta stories are created using a simple DSL and can be run interactively like a game dialog. 
* The meta stories are based on state machines that represent all state in the meta story. 
* The tool is designed to be used from the beginning of the development process to the end and can be used by all roles in the process. 
* The meta stories can be used to model a program before it is written, to validate a program after it is completed, to analyse bugs, to verify that user stories are coherent, to explore unthought of possibilities, to suggest new meta stories, to analyse bugs
* This tool relies on an intermédiate artifact (called a meta story) that acts as the glue between all stages of the software development cycle and can in theory be used by any actor, however technical, in the process.
* This intermediate artifact is a simplified model of the program that is humanly comprehensible and can be used to analyse, comprehend and strengthen a project.
* The tool is designed to be used in a shift left methodology, to model a program before it is written and to validate it after it is written.
* A behaviour modelling and requirements validation tool.
* A minimal DSL allows for the quick creation of meta stories.
* A game dialog like interface allows the user to interact with their meta stories.
* State machines are used to represent all state in a meta stories which allows for both easy comprehension and exhaustive analysis.

# Specific nomenclature

## Story

run/execution/conversation, could almost be called a user story except that name is already taken but is very similar and can be associated with one

## Meta story

Scenario/encapsulates all possible stories for a situation


# Advantages

* The model is linked strongly to the code to ensure correctness.
* Dialogue can be rewound easily unlike code execution.
* Limited state machines that represent values and configuration force modelling of only the most important of information.
* An entire story or meta story can be shared and explored statically allowing for better requirement communication, or debugging.
* Model can be analysed separately from code.
* Model can be run like a dialogue independently of program for ease of comprehension.
* Can be run by anyone not just a developer. A product owner, a tester, or support can test meta stories to predict behaviour.
* State machines as values might allow for some level of exhaustive analysis.
* We have a client that with A po develops a set of user stories. Gives them a new way to exchange and validate the requirements in a more reliable way. Allows interaction by all parties (client, po, Dev, test).
* Verification that user stories are coherent.
* Explore unthought of possibilities before any code is written.
* Probability of states and progressive accumulation allow for outcome assessment.
* Modelling can be done at high or low level.
* Generation of new story possibility to catch edge cases.
* Shift left tooling to model a program before conception as well as validate it after completion.
* Backtracking analysis allows for bug analysis.
* It's like a game dialogue, it's fun and accessible. Unlike most functional requirements ':D
* Strong verifiable link to code
    * Construction and validation using hooks
    * Construction and validation directly from traces/logs
    * Progressive generation allows for building a meta story bit by bit from several executions or trace sets, each time adding new parts and checking validity against what's already been established
    * Could be used in TDD
    * Coverage check over all tests to make sure that a maximum of the meta story is covered by tests

# Disadvantages

* Introduces a some coupling between the model and the code which could be seen as depending on implementation details, which is generally seen as negative. The main coupling concern is on flow and large scale structuring of the meta story and application.
    * A future development could be to maintain two versions of the model, a high level version that is oriented client requirements and general comprehension and the other low level that is more coupled to the code base, with an automatic validation between the two that verifies that the main user stories are still coherent.
* YASDT (Yet Another Software Development Tool) to add to the pile.
* Requires a lot of work to model a complex system.
    * This is still work that has to be done anyway but is usually done in the form of functional requirements or user stories.

# Use cases
* Systems with lots of configuration and state, and/or very complex logic that makes mentally running through the execution difficult.
* Shift left : conception to maintenance Requirement Traceability Tooling
* Adding an extra level of logical non regression testing to integration tests
* Enabling non technical roles like PO to model processes or to validate intended behaviour

# Features and notable aspects

* Builds a simplified model of the program/scenario with finite possibilities.
* Not Turing complete which helps analysis, but just complete enough so as to be able to model complex scenarios and be humanly comprehensible.
* Model abstracts out fine implementation details, like extraneous code, stack information, etc so that the meta story can concentrate on the desired logic.
* Stories and meta stories are human readable allowing for manual construction and modification, but can also be manipulated automatically in several ways.


# Usage

## Web UI

All data is stored in the URL of the page like PlantUML. This allows for easy sharing of meta stories. Just copy the URL and send it to a friend.

To the left is the meta story editor. Here you can write your meta stories using the DSL.

To the right is the meta story runner. Here you can interact with your meta story like a game dialog or run an exhaustivity analysis to find weak points in the meta story conception.

## Integrating into an existing code base via hooks

<TODO>

# Manual interaction

<TODO>

## Automated analysis

<TODO>


# The DSL

The DSL (Domain Specific Language) used is a simple custom language that can describe a logical system and and one or more meta stories.

In a system the following concepts are available to describe a system in which a story may happen:
* Entities
* Aspects (on entities)
* States (that apply to entities, aspects or relations)
* Transitions between states
* Relations (between entities, or between aspects or between entities and aspects)
* Constraints (on anything)

The meta stories may have the following types of steps:
* Dialog
* Transition
* If
* Choose
* While
* Jump

These are described in more detail below.

## Entities and aspects

An entity can represent a character, a place, or an object, anything you want just as long as you can name it. 

An entity is never created or destroyed, it will exist throughout the lifetime of a meta story.

An entity may also have a set of aspects. An aspect is a property of an entity that may itself have its own state machine. Aspects allows for more complex entities.

Defining an entity is done as follows 

```
Entity Jean-Luc
```

Aspects may be created as follows

```
Entity Jean-Luc
{
	Aspect Rank
}
```

## State machines (states and transitions)

States along with defined transitions between the states make up a state machine. 

A state machine is a representation of the possible states an entity or an aspect or a relation can be in and the possible transitions between those states.

State machines are defined as follows

```
SM "Possible Ranks"
{
	Cadet -> Ensign
	Ensign -> Lieutenant
	Lieutenant -> Commander
	Commander -> Capitain
	Capitain -> Admiral
}
```

An initial state may be applied to an entity or aspect as follows

```
Entity Jean-Luc
{
	Aspect Rank
	{
		State Captain
	}
}
```

## Relations

Relations are connections between entities or between aspects or between entities and aspects.

Relations can have a state just like entities and aspects.

Relations are defined in their simplest form as follows

```
Jean-Luc -> Enterprise
```

More detail can be added to a relation as follows

```
Jean-Luc -> Enterprise : "Assigned To"
{
	State "Currently On Leave"
}
```

## Constraints

Constraints are rules that must always be true in a meta story. They can be used to ensure that your meta story remains valid according to how you imagine it.

They can apply to entities, aspects, states, transitions or relations.

They are defined as follows

```
Constraint "Jean-Luc is always related to the Enterprise in some way"
{
	Relation exists : Jean-Luc -> Enterprise
}

Constraint "Jean-Luc is always a Capitain"
{
	Jean-Luc.Rank = Capitain
}
```

## Dialog

<TODO>

## Transition

<TODO>

## If

<TODO>

## Choose

<TODO>

## While

<TODO>

## Jump

<TODO>

## Example

In the following meta story, we have a system that describes an actor that has a name. Their name is part of their state and so may change according to the associated state machine. The state machine allows for a simple progression from one name to the next covering a total of 4 names.

The meta story reads as follows :
* The entity Actor starts with an intial name
* and as the loop plays out, the actor's name is swapped out for a new one
* Each time the name is swapped the actor says something related to their name.

```
Entity Actor {
    EntityType ET1
    State "Amy Stake"
    CharacterStyle Red
}
        
EntityType ET1 {
    SM Name
}
        
SM Name {
    State "Amy Stake"
    State "Brock Lee"
    State "Clara Nett"
    State "Dee Zaster"
    "Amy Stake" -> "Brock Lee" : ChangeName
    "Brock Lee" -> "Clara Nett" : ChangeName
    "Clara Nett" -> "Dee Zaster" : ChangeName
}
        
MetaStory "Face off" {
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
```

# Next steps

## Event sourcing

State : partial implementation

* Event sourcing for run through stories and replaying

## UML Diagram export

State : not implemented

* Export to UML diagrams like sequence diagrams, state diagrams, activity diagrams, etc

## Unit test intégration

State : partial implementation

* * Each TU adds to a named meta story so that it can represent several paths through the execution
* A commun system is defined
* The meta story is tested at the end of the TU against it's initial state and assertions can be made against it's final state
* Static functions positioned throughout the production code allow for both initial construction of the meta stories and, on subsequent runs, validation on the already established meta stories
* Declare state transitions
* Declare if condition, along with whether the condition was true or false.
* Or loop start and end, along with weather the loop continued or ended
* Declare dialogue by actors in code like traces
* Hook code could throw exceptions in TU instead of at end, so that you can debug the meta story at the same time as debugging the code to ensure correctness and facilitate comprehension. For example on a constraint violation or an impossible state transition.
* Visual studio add-on to show state machine and entities as execution progresses
* Multiple choice steps could derive from having multiple state transitions possible

## Implicit graph nodes

State : Partial implementation

* Implicit graph nodes that are not necessary in some circumstances so that there is some tolerance but only in specific places of the meta story
* Would allow for construction of meta stories via traces because not all traces are necessaily present because of differing trace levels

## Probabilistic state machines

State : not implemented

* Probabilistic state machines for story generation and analysis

## Web interface

State : not implemented

* Editor
* Diagram export
* Analysis tooling
* Interactive dialogue
    * Allows for rewinding
* State machine animations visually attached to entities aspects relations etc
* List all contraint-transition pairs that could block at each step
* State machines visualised beside the story dialog
* The server side of the web interface is only used to serve the static files and does not store any data, nor does data get sent to the server, which is ideal from a security standpoint.

## Meta stories within meta stories

State : not implemented

* A meta story can incorporate another, so that parts of meta story can be built separately and incorporated afterwards
    * Useful for when the meta story is spread over several products like micro services 

## Meta story coverage

State : not implemented

* Meta story code coverage equivalent

## Equivalency between meta stories

State : not implemented

* Possibility to have two versions of a meta story
    * a high level version that is oriented client requirements 
    * and the other a low level version that is more coupled to the code base and its implementation details
    * with an automatic validation between the two that verifies that the main user stories are coherent

## Exhaustivity

State : not implemented

* Have all possibilities been modelled by a meta story?
* Use AI to build a meta story that hasn't yet been modelled
* Not sure if possible like this
* Analysis of meta stories using exhaustive search in the possibility space
	* Discovery of unmentionned paths through a meta story

## New meta story suggestions

State : not implemented

* If the exhaustivity idea can't work
* Try suggesting the beginning of a new meta story that hasn't been done yet or the continuation of an existing one.

## Backtracking satisfiability analysis

State : not implemented

* Start from final story state and search for a list of possible starting states that could produce it
* That is given a bug, a final undesirable state, is there a path through the meta story that could produce it? And what would be the initial conditions of that story?
* Technical idea : working backwards, state changes in reverse, if blocks add constraints may be necessary to create new variables for new scopes, loops like repeated if blocks
* Sat solver ?
    * https://youtu.be/6OPsH8PK7xM
    * https://github.com/lifebeyondfife/Decider
