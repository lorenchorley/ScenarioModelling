# Warning

This project is still in its experimental stage.

# TL;DR

This tool provides a strong verifiable link between the functional requirements and the final implmentation of a product.

It does so through the use of an intermediate artifact that acts as a bridge between all stages of the software development lifecycle, and encapsulates the essentiel logic required to respond to the functional requirements.

This intermediate artifact is humanly comprehensible like a UML diagram and can be used to automatically or manually analyse a product, and even improve it through bug analysis tooling.


# Full introduction

It is a tool that allows for the creation of scenarios that can be run and analysed. 

The scenarios are created using a simple DSL and can be run interactively like a game dialog. 

The scenarios are based on state machines that represent all state in the scenario. 

The tool is designed to be used from the beginning of the development process to the end and can be used by all roles in the process. 

The scenarios can be used to model a program before it is written, to validate a program after it is completed, to analyse bugs, to verify that user stories are coherent, to explore unthought of possibilities, to suggest new scenarios, to analyse bugs

This tool relies on an intermédiate artifact (called a meta story) that acts as the glue between all stages of the software development cycle and can in theory be used by any actor, however technical, in the process.

This intermediate artifact is a simplified model of the program that is humanly comprehensible and can be used to analyse, comprehend and strengthen a project.

The tool is designed to be used in a shift left methodology, to model a program before it is written and to validate it after it is written.

A behaviour modelling and requirements validation tool.

A minimal DSL allows for the quick creation of scenarios.

A game dialog like interface allows the user to interact with their scenarios.

State machines are used to represent all state in a scenario which allows for both easy comprehension and exhaustive analysis.

## Specific nomenclature

Story = run/execution/conversation, could almost be called a user story except that name is already taken but is very similar and can be associated with one

Meta story = scenario/encapsulates all possible stories for a situation

## Motivation

* Lack of strong tooling that goes from the beginning of the process at client requirement gathering to the end at maintenance, that is accessible and useful for every role in the process.
* Compliments the standard documentation, development and testing
* Provides a strong, automatically verifiable, link between requirements and code
    * Enables a developer to immdeiately locate the code that responds to a given functional requirement and vise versa
* Improved exploration of the possibility space in client requirements gathering, debugging and testing
* Interactive, comprehensible, analysable
* Exportable into standard artefacts like UML diagrams
* Can be put in place after development to analyse, comprehend and strengthen a legacy project

### Advantages

* Builds a simplified model of the program/scenario with finite possibilities
* Not Turing complete which helps analysis, but just complete enough so as to be able to model complex scenarios and be humanly comprehensible
* Model abstracts out unwanted implementation details, like extraneous code, stack information, etc so that the scenario can concentrate on the desired logic
* Model can be analysed separately from code
* Model can be run like a dialogue independently of program for ease of comprehension
* Dialogue can be rewound easily unlike code execution
* An entire execution can be shared and explored statically allowing for better requirement communication, or debugging
* Can be run by anyone not just a développeur, even a product owner can test scenarios to predict behaviour
* The model is linked strongly to the code to ensure correctness
* Limited state machines that represent values and configuration force modelling of only the most important of information
* State machines as values might allow for some level of exhaustive analysis
* Scenarios are human readable allowing for manual construction and edits, but can also be purely automatic with in code hooks, out a mix of both
* We have a client that with A po develops a set of user stories. Gives them a new way to exchange and validate the requirements in a more reliable way. Allows interaction by all parties (client, po, Dev, test)
* Verification that user stories are coherent
* Explore unthought of possibilities before any code is written
* Probability of states and progressive accumulation allow for outcome assessment
* Modelling can be done at high or low level, on interface interactions or plain execution
* Generation of new scenario possibility to catch edge cases
* Shift left tooling to model a program before conception and also validate it after completion
* Backtracking analysis allows for bug analysis
* It's like a game dialogue, it's fun. Unlike functional requirements

### Disadvantages

* Requires a lot of work to model a complex system
    * This is still work that has to be done anyway but is usually done in the form of functional requirements or user stories
* Introduces a soft coupling between the model and the code which could be seen as depending on implementation details. The main coupling is on flow and large scale structing of the 
    * A future development could be to maintain two versions of the model, a high level version that is oriented client requirements and general comprehension and the other low level that is more coupled to the code base, with an automatic validation between the two that verifies that the main user stories are still coherent
* YASDT (Yet Another Software Development Tool) to add to the pile

### Strong verifiable link to code
* Construction and validation using hooks
* Construction and validation directly from traces/logs
* Progressive generation allows for building a scenario bit by bit from several executions or trace sets, each time adding new parts and checking validity against what's already been established
* Could be used in TDD
* Coverage check over all tests to make sure that a maximum of the scenario is covered by tests

### Use cases
* Systems with lots of configuration and state, and/or very complex logic that makes mentally running through the execution difficult.
* Shift left : conception to maintenance Requirement Traceability Tooling
* Adding an extra level of logical non regression testing to integration tests
* Enabling non technical roles like PO to model processes or to validate intended behaviour

## Usage

### Sharing

All data is stored in the URL of the page like PlantUML. This allows for easy sharing of scenarios. Just copy the URL and send it to a friend.

### Editing

To the left is the scenario editor. Here you can write your scenario using the DSL. The scenario is automatically updated as you type.

### Running

To the right is the scenario runner. Here you can interact with your scenario like a game dialog or run an exhaustivity analysis to find weak points in the scenario conception.


## The DSL

DSL stands for Domain Specific Language. The ScenarioModelling DSL is a simple language that allows for the creation of a logical system and scenarios that may be played out on that system.

In the DSL you can define the following concepts:
	* Entities
	* Aspects (on entities)
	* States (that apply to entities, aspects or relations)
	* Transitions between states
	* Relations (between entities, or between aspects or between entities and aspects)
	* Constraints (on anything)

### Entities and aspects

An entity can represent a character, a place, or an object, anything you want just as long as you can name it. 

An entity is never created or destroyed, it will exist throughout the lifetime of a scenario.

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

### State machines (states and transitions)

States along with defined transitions between the states make up a state machine. 

A state machine is a representation of the possible states an entity or an aspect or a relation can be in and the possible transitions between those states.

State machines are defined as follows

```
SM PossibleRanks
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
		State Capitain
	}
}
```

### Relations

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

### Constraints

Constraints are rules that must always be true in a scenario. They can be used to ensure that your scenario remains valid according to how you imagine it.

They can apply to entities, aspects, states, transitions or relations.

They are defined as follows

```
Constraint "Jean-Luc is always related to the Enterprise in some way"
{
	Relation exists : Jean-Luc -> Enterprise
}

Constraint "Jean-Luc is always a Capitain"
{
	Jean-Luc.Rank is Capitain
}
```

## Next steps

	
	* Event sourcing for scenario execution and replay
	* Probabilistic state machines for scenario generation
	* Analysis of scenarios using exhaustive search in the possibility space
		* Discovery of unmentionned paths through a scenario
	* Two ways to execute a scenario: 
		* As an automated simulation that can calculate fuzzy paths through the execution
		* As a manual run that resembles a game dialog
	* Export to UML diagrams
	* Construction of meta stories via traces
	* Integration with unit tests
	* Meta story code coverage equivalent
	* Implicit graph nodes that are not necessary in some circumstances so that there is some tolerance to missing traces for example because of differing trace levels
	* Probabilistic state machines and logic branches
		* Probabilistic analysis of scenarios
	* Graphs of a meta story that can incorporate other graphs, so that the parts of meta story can be built separately and incorporated afterwards
	* Possibility to have two versions of a meta story, a high level version that is oriented client requirements and the other a low level version that is more coupled to the code base and its implementation details, with an automatic validation between the two that verifies that the main user stories are still coherent

### Web interface

* Editor
* Diagram export
* Analysis tooling
* Interactive dialogue
    * Allows for rewinding
* State machine animations visually attached to entities aspects relations etc
* List all contraint-transition pairs that could block at each step
* State machines visualised beside the story dialog
* The server side of the web interface is only used to serve the static files and does not store any data, nor does data get sent to the server, which is ideal from a security standpoint.

### Unit test intégration

* Each TU adds to a named scenario so that it can represent several paths through the execution
* A commun system is defined
* The scenario is tested at the end of the TU against it's initial state and assertions can be made against it's final state
* Static functions positioned throughout the production code allow for both initial construction of the scenarios and, on subsequent runs, validation on the already established scenarios
* Declare state transitions
* Declare if condition, along with whether the condition was true or false.
* Or loop start and end, along with weather the loop continued or ended
* Declare dialogue by actors in code like traces
* Hook code could throw exceptions in TU instead of at end, so that you can debug the scenario at the same time as debugging the code to ensure correctness and facilitate comprehension. For example on a constraint violation or an impossible state transition.
* Visual studio add-on to show state machine and entities as execution progresses
* Multiple choice steps could derive from having multiple state transitions possible

### Is there any way to end up at a certain configuration?
* That is given a bug, a final undesirable state, is there a path through a scenario that could result in this? And what would be the initial conditions of that scenario?
* https://youtu.be/6OPsH8PK7xM
* https://github.com/lifebeyondfife/Decider

### Exhaustivity

* Have all possibilities been modelled by a scenario?
* Use ai to build a scenario that hasn't yet been modelled
* Not sure if possible like this

### New scenario suggestions

* If the exhaustivity idea can't work
* Try suggesting the beginning of a new scenario that hasn't been done yet or the continuation of an existing one.

### Backtracking satisfiability analysis

* Start from final scenario state and give a list of possible starting scenarios that could produce it
* Sat solver
* Working backwards, state changes in reverse, if blocks add constraints may be necessary to create new variables for new scopes, loops like repeated if blocks
