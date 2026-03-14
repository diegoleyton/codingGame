# Programming Game Engine

A small execution engine for building **instruction-based games** where a
program is composed of steps that operate on a game world.

The goal of this project is to support games where users assemble programs
using instructions and execute them step-by-step.

This pattern is similar to programming puzzle games like:

- Scratch
- Code.org puzzles
- Human Resource Machine
- Lego Mindstorms

But the engine itself is **domain independent**.

---

# Architecture

![Architecture](docs/diagrams/coreArchitecture.svg)

The system is organized around four main concepts:

- **Game**
- **Instruction Definitions**
- **Instruction Instances**
- **Program Runner**

---

# Game

A game implements the `IGame` interface.

Responsibilities:

- Represents the **state of the world**
- Defines the **available instruction definitions**
- Exposes operations that instructions will call

Example domain:


MovingGame
├─ MoveForward
├─ RotateLeft
└─ RotateRight


Each game decides which instructions are available.

This allows completely different games to reuse the same execution engine.

---

# Instruction Definitions

Instruction definitions describe **what an instruction does**, but they do
not represent a specific use of that instruction.

They define:

- Identifier
- Display name
- Parameters
- Default parameter values
- Execution logic
- Whether the instruction supports children

Examples:


MoveForwardInstructionDefinition
RotateLeftInstructionDefinition
RepeatInstructionDefinition
SequenceInstructionDefinition


Instruction definitions are provided by the game.

---

# Instruction Instances

An `InstructionInstance` represents **a concrete instruction used in a program**.

It contains:

- A reference to its `InstructionDefinition`
- Parameter values chosen by the user
- Optional child instructions

Example:


MoveForward(steps = 3)


Multiple instruction instances can reference the same definition.

---

# Program Definition

A `ProgramDefinition` represents the **structure of a program**.

It is essentially a tree of instruction instances.

Example:


Repeat(3)
└─ MoveForward(1)


The program stores:

- structure
- parameter values

But it does **not** contain execution logic.

---

# Program Runner

The `ProgramRunner` executes programs step-by-step.

Responsibilities:

- Traverses the program
- Maintains execution state using `ExecutionFrame`
- Executes primitive instructions
- Returns a `StepResult` after each step

This enables:

- step-by-step execution
- animations
- debugging
- educational visualizations

---

# Execution Flow

![Execution Flow](docs/diagrams/coreFlow.svg)

Execution works roughly like this:

1. A `Game` is created.
2. The game provides available instruction definitions.
3. The user builds a program using instruction instances.
4. The program is loaded into the `ProgramRunner`.
5. Each call to `ExecuteNextStep()`:
   - retrieves the next instruction
   - resolves its instruction definition
   - executes it on the game
6. A `StepResult` is returned.

---

# Example

Example program for a maze game:


Repeat(3)
└─ MoveForward(1)
RotateLeft()
MoveForward(1)


Execution would proceed step-by-step:


1 → MoveForward
2 → MoveForward
3 → MoveForward
4 → RotateLeft
5 → MoveForward


Each step updates the game state.

---

# Design Goals

## Separation of Concerns

Instruction definitions describe **behavior**.

Instruction instances describe **how that behavior is used in a program**.

---

## Extensibility

New games only need to define:

- a new `IGame`
- a set of instruction definitions

The execution engine does not need to change.

---

## Step-based Execution

Programs execute one instruction at a time.

This makes it easy to support:

- animations
- debugging
- educational tools
- visual execution traces

---

## Domain Independence

The engine does not know about game rules.

All domain logic lives inside:

- the game
- the instruction definitions

---

# Project Structure


Core/
ProgramRunner
ProgramDefinition
ExecutionFrame
InstructionInstance
InstructionParameterDefinition

Instructions/
InstructionDefinitionBase
SequenceInstructionDefinition
RepeatInstructionDefinition

Games/
MovingGame
MoveForwardInstructionDefinition
RotateLeftInstructionDefinition
RotateRightInstructionDefinition


---

# Diagrams

Architecture diagrams are written using **PlantUML** and exported as SVG.

They are located in:


docs/diagrams


These diagrams describe:

- the system architecture
- the execution flow
