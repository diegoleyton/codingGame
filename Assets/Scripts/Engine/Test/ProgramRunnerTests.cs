using System;
using NUnit.Framework;
using Flowbit.Engine;
using Flowbit.Engine.Instructions;

/// <summary>
/// Contains unit tests for the program runner and instruction definition system.
/// </summary>
public sealed class ProgramRunnerTests
{
    [Test]
    public void InstructionInstance_UsesDefaultParameterValues()
    {
        MockPrimitiveInstructionDefinition definition =
            new MockPrimitiveInstructionDefinition(
                id: "counting_instruction",
                displayName: "Counting Instruction",
                logEntry: "Count",
                supportsAmountParameter: true);

        InstructionInstance instance = new InstructionInstance(definition);

        Assert.AreEqual(1, instance.GetParameterValue("amount"));
    }

    [Test]
    public void InstructionInstance_SetParameterValue_OverridesDefault()
    {
        MockPrimitiveInstructionDefinition definition =
            new MockPrimitiveInstructionDefinition(
                id: "counting_instruction",
                displayName: "Counting Instruction",
                logEntry: "Count",
                supportsAmountParameter: true);

        InstructionInstance instance = new InstructionInstance(definition);
        instance.SetParameterValue("amount", 3);

        Assert.AreEqual(3, instance.GetParameterValue("amount"));
    }

    [Test]
    public void ProgramDefinition_AddNullInstruction_Throws()
    {
        ProgramDefinition program = new ProgramDefinition();

        Assert.Throws<ArgumentNullException>(() => program.AddInstruction(null));
    }

    [Test]
    public void Runner_ExecuteWithoutProgram_Throws()
    {
        ProgramRunner runner = new ProgramRunner();
        GameMock game = new GameMock();

        Assert.Throws<InvalidOperationException>(() => runner.ExecuteNextStep(game));
    }

    [Test]
    public void Runner_ExecuteWithNullGame_Throws()
    {
        ProgramRunner runner = new ProgramRunner();
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "turn_left",
                displayName: "Turn Left",
                logEntry: "TurnLeft")));

        runner.LoadProgram(program);

        Assert.Throws<ArgumentNullException>(() => runner.ExecuteNextStep(null));
    }

    [Test]
    public void Runner_WithSinglePrimitive_ExecutesInstruction()
    {
        ProgramDefinition program = new ProgramDefinition();

        MockPrimitiveInstructionDefinition definition =
            new MockPrimitiveInstructionDefinition(
                id: "counting_instruction",
                displayName: "Counting Instruction",
                logEntry: "Count",
                supportsAmountParameter: true);

        InstructionInstance instruction = new InstructionInstance(definition);
        instruction.SetParameterValue("amount", 3);
        program.AddInstruction(instruction);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        StepResult result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, result.GetStatus());
        Assert.AreSame(instruction, result.GetExecutedInstruction());
        CollectionAssert.AreEqual(new[] { "Count(3)" }, game.GetLog());
    }

    [Test]
    public void Runner_AfterLastInstruction_ReturnsCompletedProgram()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "turn_left",
                displayName: "Turn Left",
                logEntry: "TurnLeft")));

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        StepResult first = runner.ExecuteNextStep(game);
        StepResult second = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, first.GetStatus());
        Assert.AreEqual(StepExecutionStatus.CompletedProgram, second.GetStatus());
        Assert.IsNull(second.GetExecutedInstruction());
    }

    [Test]
    public void Runner_WhenGameAlreadyWon_ReturnsBlockedAndDoesNotExecute()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "log_instruction",
                displayName: "Log Instruction",
                logEntry: "Log")));

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();
        game.SetWon(true);

        StepResult result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.BlockedByGameState, result.GetStatus());
        Assert.IsEmpty(game.GetLog());
    }

    [Test]
    public void Runner_WhenGameAlreadyFailed_ReturnsBlockedAndDoesNotExecute()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "log_instruction",
                displayName: "Log Instruction",
                logEntry: "Log")));

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();
        game.SetFailed(true);

        StepResult result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.BlockedByGameState, result.GetStatus());
        Assert.IsEmpty(game.GetLog());
    }

    [Test]
    public void SequenceInstruction_ExecutesChildrenOnePrimitivePerStep()
    {
        InstructionInstance sequence = new InstructionInstance(new SequenceInstructionDefinition());

        InstructionInstance instruction1 = new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "first",
                displayName: "First",
                logEntry: "First"));

        InstructionInstance instruction2 = new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "second",
                displayName: "Second",
                logEntry: "Second"));

        InstructionInstance instruction3 = new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "third_counting",
                displayName: "Third Counting",
                logEntry: "Third",
                supportsAmountParameter: true));

        instruction3.SetParameterValue("amount", 2);

        sequence.AddChild(instruction1);
        sequence.AddChild(instruction2);
        sequence.AddChild(instruction3);

        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(sequence);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        StepResult step1 = runner.ExecuteNextStep(game);
        StepResult step2 = runner.ExecuteNextStep(game);
        StepResult step3 = runner.ExecuteNextStep(game);
        StepResult step4 = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, step1.GetStatus());
        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, step2.GetStatus());
        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, step3.GetStatus());
        Assert.AreEqual(StepExecutionStatus.CompletedProgram, step4.GetStatus());

        CollectionAssert.AreEqual(
            new[] { "First", "Second", "Third(2)" },
            game.GetLog());
    }

    [Test]
    public void RepeatInstruction_ExpandsAndExecutesRepeatedChildren()
    {
        InstructionInstance repeat = new InstructionInstance(new RepeatInstructionDefinition());
        repeat.SetParameterValue("count", 3);

        repeat.AddChild(new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha")));

        InstructionInstance countingInstruction = new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "beta",
                displayName: "Beta",
                logEntry: "Beta",
                supportsAmountParameter: true));

        countingInstruction.SetParameterValue("amount", 1);
        repeat.AddChild(countingInstruction);

        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(repeat);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        for (int i = 0; i < 6; i++)
        {
            runner.ExecuteNextStep(game);
        }

        StepResult completed = runner.ExecuteNextStep(game);

        CollectionAssert.AreEqual(
            new[]
            {
                "Alpha", "Beta(1)",
                "Alpha", "Beta(1)",
                "Alpha", "Beta(1)"
            },
            game.GetLog());

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, completed.GetStatus());
    }

    [Test]
    public void NestedComposite_ExecutesInCorrectOrder()
    {
        InstructionInstance nestedSequence = new InstructionInstance(new SequenceInstructionDefinition());

        nestedSequence.AddChild(new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "turn",
                displayName: "Turn",
                logEntry: "Turn")));

        InstructionInstance moveTwo = new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "move_two",
                displayName: "Move Two",
                logEntry: "Move",
                supportsAmountParameter: true));

        moveTwo.SetParameterValue("amount", 2);
        nestedSequence.AddChild(moveTwo);

        InstructionInstance repeat = new InstructionInstance(new RepeatInstructionDefinition());
        repeat.SetParameterValue("count", 2);

        InstructionInstance moveOne = new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "move_one",
                displayName: "Move One",
                logEntry: "Move",
                supportsAmountParameter: true));

        moveOne.SetParameterValue("amount", 1);

        repeat.AddChild(moveOne);
        repeat.AddChild(nestedSequence);

        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(repeat);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        for (int i = 0; i < 6; i++)
        {
            runner.ExecuteNextStep(game);
        }

        StepResult final = runner.ExecuteNextStep(game);

        CollectionAssert.AreEqual(
            new[]
            {
                "Move(1)",
                "Turn",
                "Move(2)",
                "Move(1)",
                "Turn",
                "Move(2)"
            },
            game.GetLog());

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, final.GetStatus());
    }

    [Test]
    public void Runner_ResetExecution_RunsProgramAgainFromStart()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha")));

        InstructionInstance countingInstruction = new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "beta",
                displayName: "Beta",
                logEntry: "Beta",
                supportsAmountParameter: true));

        countingInstruction.SetParameterValue("amount", 1);
        program.AddInstruction(countingInstruction);

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        runner.ExecuteNextStep(game);
        runner.ResetExecution();
        game.ResetGame();

        runner.ExecuteNextStep(game);
        runner.ExecuteNextStep(game);

        CollectionAssert.AreEqual(
            new[] { "Alpha", "Beta(1)" },
            game.GetLog());
    }

    [Test]
    public void Runner_IsFinished_BehavesCorrectly()
    {
        ProgramRunner runner = new ProgramRunner();
        Assert.IsTrue(runner.IsFinished());

        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha")));

        runner.LoadProgram(program);
        Assert.IsFalse(runner.IsFinished());

        GameMock game = new GameMock();
        runner.ExecuteNextStep(game);
        Assert.IsFalse(runner.IsFinished());

        runner.ExecuteNextStep(game);
        Assert.IsTrue(runner.IsFinished());
    }

    [Test]
    public void Runner_WithEmptyProgram_CompletesImmediately()
    {
        ProgramDefinition program = new ProgramDefinition();

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        StepResult result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, result.GetStatus());
        Assert.IsNull(result.GetExecutedInstruction());
    }

    [Test]
    public void Runner_AfterCompletion_StaysCompleted()
    {
        ProgramDefinition program = new ProgramDefinition();
        program.AddInstruction(new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha")));

        ProgramRunner runner = new ProgramRunner();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        runner.ExecuteNextStep(game);

        StepResult result1 = runner.ExecuteNextStep(game);
        StepResult result2 = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, result1.GetStatus());
        Assert.AreEqual(StepExecutionStatus.CompletedProgram, result2.GetStatus());
    }

    [Test]
    public void InstructionInstance_AddChild_ToPrimitiveInstruction_Throws()
    {
        InstructionInstance instruction = new InstructionInstance(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha"));

        Assert.Throws<InvalidOperationException>(() =>
            instruction.AddChild(new InstructionInstance(
                new MockPrimitiveInstructionDefinition(
                    id: "beta",
                    displayName: "Beta",
                    logEntry: "Beta"))));
    }
}