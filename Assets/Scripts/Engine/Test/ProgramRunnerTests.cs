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

        InstructionInstance<int> instance = new InstructionInstance<int>(definition);

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

        InstructionInstance<int> instance = new InstructionInstance<int>(definition);
        instance.SetParameterValue("amount", 3);

        Assert.AreEqual(3, instance.GetParameterValue("amount"));
    }

    [Test]
    public void ProgramDefinition_AddNullInstruction_Throws()
    {
        ProgramDefinition<int> program = new ProgramDefinition<int>();

        Assert.Throws<ArgumentNullException>(() => program.AddInstruction(null));
    }

    [Test]
    public void Runner_ExecuteWithoutProgram_Throws()
    {
        ProgramRunner<int> runner = new ProgramRunner<int>();
        GameMock game = new GameMock();

        Assert.Throws<InvalidOperationException>(() => runner.ExecuteNextStep(game));
    }

    [Test]
    public void Runner_ExecuteWithNullGame_Throws()
    {
        ProgramRunner<int> runner = new ProgramRunner<int>();
        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(new InstructionInstance<int>(
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
        ProgramDefinition<int> program = new ProgramDefinition<int>();

        MockPrimitiveInstructionDefinition definition =
            new MockPrimitiveInstructionDefinition(
                id: "counting_instruction",
                displayName: "Counting Instruction",
                logEntry: "Count",
                supportsAmountParameter: true);

        InstructionInstance<int> instruction = new InstructionInstance<int>(definition);
        instruction.SetParameterValue("amount", 3);
        program.AddInstruction(instruction);

        ProgramRunner<int> runner = new ProgramRunner<int>();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        StepResult<int> result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, result.GetStatus());
        Assert.AreSame(instruction, result.GetExecutedInstruction());
        CollectionAssert.AreEqual(new[] { "Count(3)" }, game.GetLog());
    }

    [Test]
    public void Runner_AfterLastInstruction_ReturnsCompletedProgram()
    {
        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "turn_left",
                displayName: "Turn Left",
                logEntry: "TurnLeft")));

        ProgramRunner<int> runner = new ProgramRunner<int>();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        StepResult<int> first = runner.ExecuteNextStep(game);
        StepResult<int> second = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.ExecutedPrimitive, first.GetStatus());
        Assert.AreEqual(StepExecutionStatus.CompletedProgram, second.GetStatus());
        Assert.IsNull(second.GetExecutedInstruction());
    }

    [Test]
    public void Runner_WhenGameAlreadyWon_ReturnsBlockedAndDoesNotExecute()
    {
        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "log_instruction",
                displayName: "Log Instruction",
                logEntry: "Log")));

        ProgramRunner<int> runner = new ProgramRunner<int>();
        runner.LoadProgram(program);

        GameMock game = new GameMock();
        game.SetWon(true);

        StepResult<int> result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.BlockedByGameState, result.GetStatus());
        Assert.IsEmpty(game.GetLog());
    }

    [Test]
    public void Runner_WhenGameAlreadyFailed_ReturnsBlockedAndDoesNotExecute()
    {
        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "log_instruction",
                displayName: "Log Instruction",
                logEntry: "Log")));

        ProgramRunner<int> runner = new ProgramRunner<int>();
        runner.LoadProgram(program);

        GameMock game = new GameMock();
        game.SetFailed(true);

        StepResult<int> result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.BlockedByGameState, result.GetStatus());
        Assert.IsEmpty(game.GetLog());
    }

    [Test]
    public void SequenceInstruction_ExecutesChildrenOnePrimitivePerStep()
    {
        InstructionInstance<int> sequence = new InstructionInstance<int>(new MockSequenceInstructionDefinition());

        InstructionInstance<int> instruction1 = new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "first",
                displayName: "First",
                logEntry: "First"));

        InstructionInstance<int> instruction2 = new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "second",
                displayName: "Second",
                logEntry: "Second"));

        InstructionInstance<int> instruction3 = new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "third_counting",
                displayName: "Third Counting",
                logEntry: "Third",
                supportsAmountParameter: true));

        instruction3.SetParameterValue("amount", 2);

        sequence.AddChild(instruction1);
        sequence.AddChild(instruction2);
        sequence.AddChild(instruction3);

        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(sequence);

        ProgramRunner<int> runner = new ProgramRunner<int>();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        StepResult<int> step1 = runner.ExecuteNextStep(game);
        StepResult<int> step2 = runner.ExecuteNextStep(game);
        StepResult<int> step3 = runner.ExecuteNextStep(game);
        StepResult<int> step4 = runner.ExecuteNextStep(game);

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
        InstructionInstance<int> repeat = new InstructionInstance<int>(new MockRepeatInstructionDefinition());
        repeat.SetParameterValue("count", 3);

        repeat.AddChild(new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha")));

        InstructionInstance<int> countingInstruction = new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "beta",
                displayName: "Beta",
                logEntry: "Beta",
                supportsAmountParameter: true));

        countingInstruction.SetParameterValue("amount", 1);
        repeat.AddChild(countingInstruction);

        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(repeat);

        ProgramRunner<int> runner = new ProgramRunner<int>();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        for (int i = 0; i < 6; i++)
        {
            runner.ExecuteNextStep(game);
        }

        StepResult<int> completed = runner.ExecuteNextStep(game);

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
        InstructionInstance<int> nestedSequence = new InstructionInstance<int>(new MockSequenceInstructionDefinition());

        nestedSequence.AddChild(new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "turn",
                displayName: "Turn",
                logEntry: "Turn")));

        InstructionInstance<int> moveTwo = new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "move_two",
                displayName: "Move Two",
                logEntry: "Move",
                supportsAmountParameter: true));

        moveTwo.SetParameterValue("amount", 2);
        nestedSequence.AddChild(moveTwo);

        InstructionInstance<int> repeat = new InstructionInstance<int>(new MockRepeatInstructionDefinition());
        repeat.SetParameterValue("count", 2);

        InstructionInstance<int> moveOne = new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "move_one",
                displayName: "Move One",
                logEntry: "Move",
                supportsAmountParameter: true));

        moveOne.SetParameterValue("amount", 1);

        repeat.AddChild(moveOne);
        repeat.AddChild(nestedSequence);

        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(repeat);

        ProgramRunner<int> runner = new ProgramRunner<int>();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        for (int i = 0; i < 6; i++)
        {
            runner.ExecuteNextStep(game);
        }

        StepResult<int> final = runner.ExecuteNextStep(game);

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
        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha")));

        InstructionInstance<int> countingInstruction = new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "beta",
                displayName: "Beta",
                logEntry: "Beta",
                supportsAmountParameter: true));

        countingInstruction.SetParameterValue("amount", 1);
        program.AddInstruction(countingInstruction);

        ProgramRunner<int> runner = new ProgramRunner<int>();
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
    public void Runner_IsStopped_BehavesCorrectly()
    {
        ProgramRunner<int> runner = new ProgramRunner<int>();
        Assert.IsTrue(runner.IsStopped());

        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha")));

        runner.LoadProgram(program);
        Assert.IsFalse(runner.IsStopped());

        GameMock game = new GameMock();
        runner.ExecuteNextStep(game);
        Assert.IsFalse(runner.IsStopped());

        runner.ExecuteNextStep(game);
        Assert.IsTrue(runner.IsStopped());
    }

    [Test]
    public void Runner_WithEmptyProgram_CompletesImmediately()
    {
        ProgramDefinition<int> program = new ProgramDefinition<int>();

        ProgramRunner<int> runner = new ProgramRunner<int>();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        StepResult<int> result = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, result.GetStatus());
        Assert.IsNull(result.GetExecutedInstruction());
    }

    [Test]
    public void Runner_AfterCompletion_StaysCompleted()
    {
        ProgramDefinition<int> program = new ProgramDefinition<int>();
        program.AddInstruction(new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha")));

        ProgramRunner<int> runner = new ProgramRunner<int>();
        runner.LoadProgram(program);

        GameMock game = new GameMock();

        runner.ExecuteNextStep(game);

        StepResult<int> result1 = runner.ExecuteNextStep(game);
        StepResult<int> result2 = runner.ExecuteNextStep(game);

        Assert.AreEqual(StepExecutionStatus.CompletedProgram, result1.GetStatus());
        Assert.AreEqual(StepExecutionStatus.CompletedProgram, result2.GetStatus());
    }

    [Test]
    public void InstructionInstance_AddChild_ToPrimitiveInstruction_Throws()
    {
        InstructionInstance<int> instruction = new InstructionInstance<int>(
            new MockPrimitiveInstructionDefinition(
                id: "alpha",
                displayName: "Alpha",
                logEntry: "Alpha"));

        Assert.Throws<InvalidOperationException>(() =>
            instruction.AddChild(new InstructionInstance<int>(
                new MockPrimitiveInstructionDefinition(
                    id: "beta",
                    displayName: "Beta",
                    logEntry: "Beta"))));
    }
}