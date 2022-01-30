# Streamer Bot Heist Game

## Features

This was inspired by the firebot heist game but with some additional features
- Multiple possible heist stories
- Random events during heists
- User interaction to resolve events during heists
- Variable rewards based on what happens in a heist

## Warning
This project has a dependency on the interface used for the CPH object in Streamer Bot. As a result it is built against a specific version of streamer bot executable and may need to be rebuilt for newer versions. This method of loading assemblies into streamer bot is not officially supported.

In order to build this solution you will have to fix the references to the streamer bot assemblies.

## Building

Build the Zerifax.Proxies project and copy the following assemblies into the root of the Streamer Bot folder:

- Zerifax.Proxies.dll
- Zerifax.Heist.dll
- Newtonsoft.Json.dll

## Streamer Bot Configuration

### Actions

Copy the exported text below and import it into the Streamer Bot actions.

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

You will have to add references to the three assembles listed above for each action.

### Commands

Setup two commands for the heist
- !heist - runs the Heist Add User action.
- ^![abcdeABCDE] - regex action that runs the Heist Action action. Additional letters can be added to support more than 5 options in user responses.

Also add a timed action that executes the Heist Timed Runner on a regular interval (e.g. 5 seconds)

### Configuration File

The heist game is driven by a configuration file. You will have to set this up in the Heist Timed Runner action. Two examples have been included with this solution.

At a root level you will find these properties:

- Cooldown: Time in seconds between the end of one heist and the start of the next
- PrepTime: Time from the first person entering a heist until it kicks off
- MinUsers: Minimum number of users to kick off a heist. Below this limit it automatically fails
- MinPoints: Minimum points for entry into the heist
- MaxPoints: Maximum points for entry into a heist
- PointsNameVariable: The variable which contains your custom points name
- PointsVariable: The user variable used to track points

- CreateTeamMessage: Chat message for the first person to enter a given heist
- UserEntryMessage: Chat message when any user enters the heist
- NotEnoughUsersMessage: Chat message when not enough users enter the heist
- CooldownMessage: Chat message for users who try and enter a heist during cooldown
- ReadyMessage: Chat message for when cooldown ends

At the root level you will find a list of possible events. For the heist to work you need at least one event. Events in this list are chosen at random when the heist starts. Events have the following properties:

- SuccessChance (0-100): Determines the chance of success for each user on an individual basis
- PointsMultiplier (decimal): Invested points by each user will be multiplied by this number to determine winnings if successful
- EventChance (0-100): This determines the chance that a subevent will be triggered

- StartMessage: Chat message when the heist event starts 
- SuccessMessage: Chat message when the entire team succeeds
- PartialSuccessMessage: Chat message when there's a mix of successful and unsuccessful users
- FailMessage: Chat message when the entire team fails

Sub events additionally have these optional properties:
- Command: The command used to trigger a sub event
- Description: The description for a sub event (intended as a preview)

The message properties support variable substitution in some circumstances. The supported variables are:
- [user]
- [points]
- [pointsName]

Refer to the examples for more details.

### Events

A basic heist needs at least one event at the root level, everything beyond that is optional. When additional events are provided they will operate in the following fashion:

- Events at root level are chosen at random. This event will then become the start of the heist
- Sub events are chosen based on the following criteria:
  - If at least one sub event has a command defined users will be presented with a choice of all sub events with a command
    - The first matching command entered will determine the next event to follow
    - If no commands are entered and at least one sub event can be found without a command, a sub event without a command will be chosen at random
    - If no commands are entered and no other sub events exist the heist will fail
  - If no sub events have commands defined a sub event will be chosen at random based on the parent event's chance for a sub event to trigger

The event tree does not currently have a limit and will continue until and end condition is hit or the tree runs out of events.


### Points System

This solution currently provisions for a points system based on streamer bot user variables such as VRFLad's points system. An interface has been provided (IPointsManager) should you wish to implement your own.

https://www.youtube.com/watch?v=VCnoT7wqNrE

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
