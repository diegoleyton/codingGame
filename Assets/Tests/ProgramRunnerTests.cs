using System;
using System.Collections.Generic;
using NUnit.Framework;
using CodingGame.Games.Moving;
using CodingGame.Core;
using CodingGame.Instructions;

/// <summary>
/// Contains unit tests for the program runner and instruction definition system.
/// </summary>
public sealed class ProgramRunnerTests
{
    /// <summary>
    /// Verifies that instruction instances start with default parameter values.
    /// </summary>
    [Test]
    public void InstructionInstance_UsesDefaultParameterValues()
    {
        MoveForwardInstructionDefinition definition = new MoveForwardInstructionDefinition();
        InstructionInstance instance = new InstructionInstance(definition);

        Assert.AreEqual(1, instance.GetParameterValue("steps"));
    }

    /// <summary>
    /// Verifies that instruction instance parameters can be overridden.
    /// </summary>
    [Test]
    public void InstructionInstance_SetParameterValue_OverridesDefault()
    {
        MoveForwardInstructionDefinition definition = new MoveForwardInstructionDefinition();
        InstructionInstance instance = new InstructionInstance(definition);

        instance.SetParameterValue("steps", 3);

        Assert.AreEqual(3, instance.GetParameterValue("steps"));
    }

    /// <summary>
    /// Verifies that adding a null instruction to a program throws.
    /// </summary>
    [Test]
    public void ProgramDefinition_AddNullInstruction_Throws()
    {
        ProgramDefinition program = new ProgramDefinition();

        Assert.Throws<ArgumentNullException>(() => program.AddInstruction(null));
    }

    /// <summary>
    /// Verifies that executing without a loaded program throws.
    /// </summary>
    [Test]
    public void Runner_ExecuteWithoutProgram_Throws()
    {
        ProgramRunner runner = new ProgramRunner();
        FakeMovingGame game = new FakeMovingGame();

        Assert.Throws<InvalidOperationException>(() => runner.ExecuteNextStep(game));
    }

    /// <summary>
    /// Verifies that executing with a null game throws.
    /// </summary>
    [Test]
    public void Runner_ExecuteWithNullGame_Throws()
    {
        ProgramRunner runner = new ProgramRunner();
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(new RotateLeftInstructionDefinition()));
        runner.LoadProgram(program);

        Assert.Throws<ArgumentNullException>(() => runner.ExecuteNextStep(null));
    }

    /// <summary>
    /// Verifies that a single primitive instruction executes correctly.
    /// </summary>
    [Test]
    public void Runner_WithSinglePrimitive_ExecutesInstruction()
    {
        ProgramDefinition program = new ProgramDefinition();

        InstructionInstance move = new InstructionInstance(new MoveForwardInstructionDefinition());
        move.SetParameterValue("steps", 3);
        program.AddInstruction(move);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();

        StepResult result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, result.GetStatus());
        Assert.AreSame(move, result.GetExecutedInstruction());
        CollectionAssert.AreEqual(new[] { "Move(3)" }, game.GetLog());
    }

    /// <summary>
    /// Verifies that the runner reports completion after the last instruction.
    /// </summary>
    [Test]
    public void Runner_AfterLastInstruction_ReturnsCompletedProgram()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(new RotateLeftInstructionDefinition()));

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();

        StepResult first = runner.ExecuteNextStep(game);
        StepResult second = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, first.GetStatus());
        Assert.AreEqual(StepExecutionStatus.CompletedProgram, second.GetStatus());
        Assert.IsNull(second.GetExecutedInstruction());
    }

    /// <summary>
    /// Verifies that execution is blocked if the game is already won.
    /// </summary>
    [Test]
    public void Runner_WhenGameAlreadyWon_ReturnsBlockedAndDoesNotExecute()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(new MoveForwardInstructionDefinition()));

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();
        game.SetWon(true);

        StepResult result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.BlockedByGameState, result.GetStatus());
        Assert.IsEmpty(game.GetLog());
    }

    /// <summary>
    /// Verifies that execution is blocked if the game is already failed.
    /// </summary>
    [Test]
    public void Runner_WhenGameAlreadyFailed_ReturnsBlockedAndDoesNotExecute()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(new MoveForwardInstructionDefinition()));

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();
        game.SetFailed(true);

        StepResult result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.BlockedByGameState, result.GetStatus());
        Assert.IsEmpty(game.GetLog());
    }

    /// <summary>
    /// Verifies that sequence instructions execute one primitive child per step.
    /// </summary>
    [Test]
    public void SequenceInstruction_ExecutesChildrenOnePrimitivePerStep()
    {
        InstructionInstance sequence = new InstructionInstance(new SequenceInstructionDefinition());

        InstructionInstance move1 = new InstructionInstance(new MoveForwardInstructionDefinition());
        move1.SetParameterValue("steps", 1);

        InstructionInstance rotate = new InstructionInstance(new RotateRightInstructionDefinition());

        InstructionInstance move2 = new InstructionInstance(new MoveForwardInstructionDefinition());
        move2.SetParameterValue("steps", 2);

        sequence.AddChild(move1);
        sequence.AddChild(rotate);
        sequence.AddChild(move2);

        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(sequence);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();

        StepResult step1 = runner.ExecuteNextStep(game);
        StepResult step2 = runner.ExecuteNextStep(game);
        StepResult step3 = runner.ExecuteNextStep(game);
        StepResult step4 = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, step1.GetStatus());
        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, step2.GetStatus());
        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, step3.GetStatus());
        Assert.AreEqual(StepExecutionStatus.CompletedProgram, step4.GetStatus());

        CollectionAssert.AreEqual(
            new[] { "Move(1)", "RotateRight", "Move(2)" },
            game.GetLog());
    }

    /// <summary>
    /// Verifies that repeat instructions expand and execute the expected number of times.
    /// </summary>
    [Test]
    public void RepeatInstruction_ExpandsAndExecutesRepeatedChildren()
    {
        InstructionInstance repeat = new InstructionInstance(new RepeatInstructionDefinition());
        repeat.SetParameterValue("count", 3);

        repeat.AddChild(new InstructionInstance(new RotateLeftInstructionDefinition()));

        InstructionInstance move = new InstructionInstance(new MoveForwardInstructionDefinition());
        move.SetParameterValue("steps", 1);
        repeat.AddChild(move);

        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(repeat);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();

        for (int i = 0; i < 6; i++)
        {
            runner.ExecuteNextStep(game);
        }

        StepResult completed = runner.ExecuteNextStep(game);

        CollectionAssert.AreEqual(
            new[]
            {
                "RotateLeft", "Move(1)",
                "RotateLeft", "Move(1)",
                "RotateLeft", "Move(1)"
            },
            game.GetLog());

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, completed.GetStatus());
    }

    /// <summary>
    /// Verifies that nested composite instructions execute in the correct order.
    /// </summary>
    [Test]
    public void NestedComposite_ExecutesInCorrectOrder()
    {
        InstructionInstance nestedSequence = new InstructionInstance(new SequenceInstructionDefinition());

        nestedSequence.AddChild(new InstructionInstance(new RotateRightInstructionDefinition()));

        InstructionInstance moveTwo = new InstructionInstance(new MoveForwardInstructionDefinition());
        moveTwo.SetParameterValue("steps", 2);
        nestedSequence.AddChild(moveTwo);

        InstructionInstance repeat = new InstructionInstance(new RepeatInstructionDefinition());
        repeat.SetParameterValue("count", 2);

        InstructionInstance moveOne = new InstructionInstance(new MoveForwardInstructionDefinition());
        moveOne.SetParameterValue("steps", 1);

        repeat.AddChild(moveOne);
        repeat.AddChild(nestedSequence);

        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(repeat);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();

        for (int i = 0; i < 6; i++)
        {
            runner.ExecuteNextStep(game);
        }

        StepResult final = runner.ExecuteNextStep(game);

        CollectionAssert.AreEqual(
            new[]
            {
                "Move(1)",
                "RotateRight",
                "Move(2)",
                "Move(1)",
                "RotateRight",
                "Move(2)"
            },
            game.GetLog());

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, final.GetStatus());
    }

    /// <summary>
    /// Verifies that resetting execution causes the program to run again from the beginning.
    /// </summary>
    [Test]
    public void Runner_ResetExecution_RunsProgramAgainFromStart()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(new RotateLeftInstructionDefinition()));

        InstructionInstance move = new InstructionInstance(new MoveForwardInstructionDefinition());
        move.SetParameterValue("steps", 1);
        program.AddInstruction(move);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();

        runner.ExecuteNextStep(game);
        runner.ResetExecution();
        game.ResetGame();

        runner.ExecuteNextStep(game);
        runner.ExecuteNextStep(game);

        CollectionAssert.AreEqual(
            new[] { "RotateLeft", "Move(1)" },
            game.GetLog());
    }

    /// <summary>
    /// Verifies that the runner finished state behaves correctly.
    /// </summary>
    [Test]
    public void Runner_IsFinished_BehavesCorrectly()
    {
        ProgramRunner runner = new ProgramRunner();
        Assert.IsTrue(runner.IsFinished());

        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(new RotateLeftInstructionDefinition()));

        runner.LoadProgram(program);
        Assert.IsFalse(runner.IsFinished());

        FakeMovingGame game = new FakeMovingGame();
        runner.ExecuteNextStep(game);
        Assert.IsFalse(runner.IsFinished());

        runner.ExecuteNextStep(game);
        Assert.IsTrue(runner.IsFinished());
    }

    /// <summary>
    /// Verifies that an empty program completes immediately.
    /// </summary>
    [Test]
    public void Runner_WithEmptyProgram_CompletesImmediately()
    {
        ProgramDefinition program = new ProgramDefinition();

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();

        StepResult result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, result.GetStatus());
        Assert.IsNull(result.GetExecutedInstruction());
    }

    /// <summary>
    /// Verifies that repeated calls after completion remain completed.
    /// </summary>
    [Test]
    public void Runner_AfterCompletion_StaysCompleted()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(new RotateLeftInstructionDefinition()));

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        FakeMovingGame game = new FakeMovingGame();

        runner.ExecuteNextStep(game);

        StepResult result1 = runner.ExecuteNextStep(game);
        StepResult result2 = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, result1.GetStatus());
        Assert.AreEqual(StepExecutionStatus.CompletedProgram, result2.GetStatus());
    }

    /// <summary>
    /// Verifies that child instructions cannot be added to definitions that do not support children.
    /// </summary>
    [Test]
    public void InstructionInstance_AddChild_ToPrimitiveInstruction_Throws()
    {
        InstructionInstance move = new InstructionInstance(new MoveForwardInstructionDefinition());

        Assert.Throws<InvalidOperationException>(() =>
            move.AddChild(new InstructionInstance(new RotateLeftInstructionDefinition())));
    }

    /// <summary>
    /// Verifies that the game exposes available instruction definitions.
    /// </summary>
    [Test]
    public void Game_ExposesAvailableInstructionDefinitions()
    {
        FakeMovingGame game = new FakeMovingGame();

        IReadOnlyList<IInstructionDefinition> definitions = game.GetAvailableInstructionDefinitions();

        Assert.IsNotNull(definitions);
        Assert.IsNotEmpty(definitions);
    }
}