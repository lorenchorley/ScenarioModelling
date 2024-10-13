# ScenarioModelling

A scenario modelling tool that operates similarly to PlantUML.

## Introduction

A minimal DSL allows for the quick creation of scenarios.

A game dialog like interface allows the user to interact with their scenarios.

State machines are used to represent all state in a scenario which allows for both easy comprehension and exhaustive analysis.


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

## Security

The server is only used to serve the static files and does not store any data, nor does data get sent to the server, which is ideal from a security standpoint.

## Ideas

	* State machines visualised in CSS beside the scenario dialog
	* Event sourcing for scenario execution and replay
	* Probabilistic state machines for scenario generation
	* Analysis of scenarios using exhaustive search in the possibility space
		* Discovery of unmentionned paths through a scenario
	* Two ways to execute a scenario: 
		* As an automated simulation that can calculate fuzzy paths through the execution
		* As a manual run that resembles a game dialog