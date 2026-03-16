using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CodingGame.Runtime.Core;
using CodingGame.Runtime.Instructions;
using CodingGame.Presentation.Games;
using CodingGame.Runtime.Definitions;

/// <summary>
/// Simple fake game used to test the controller.
/// </summary>
public sealed class TestGame : IGame
{
    private bool hasWon_;
    private bool hasFailed_;

    public int TotalExecutedInstructions { get; private set; }
    public int ResetCallCount { get; private set; }

    public bool HasWon()
    {
        return hasWon_;
    }

    public bool HasFailed()
    {
        return hasFailed_;
    }

    public void ResetGame()
    {
        hasWon_ = false;
        hasFailed_ = false;
        ResetCallCount++;
    }

    public void MarkWon()
    {
        hasWon_ = true;
    }

    public void MarkFailed()
    {
        hasFailed_ = true;
    }

    public void RegisterExecution()
    {
        TotalExecutedInstructions++;
    }
}

/// <summary>
/// Simple primitive instruction used by tests.
/// </summary>
public sealed class TestInstructionDefinition
    : GameInstructionDefinitionBase<TestGame>
{
    public override InstructionType GetInstructionType()
    {
        return InstructionType.MoveForward;
    }

    public override string GetDisplayName()
    {
        return "Test Instruction";
    }

    public override bool IsPrimitive()
    {
        return true;
    }

    protected override void ExecuteTyped(TestGame game, InstructionInstance instance)
    {
        game.RegisterExecution();
    }
}

/// <summary>
/// Concrete controller used to test GameControllerBase behavior.
/// </summary>
public sealed class TestGameController : GameControllerBase<TestGame>
{
    private TestGame createdGame_;

    public int InitializeViewCallCount { get; private set; }
    public int RefreshImmediateCallCount { get; private set; }
    public int RefreshAnimatedCallCount { get; private set; }
    public int ClearHighlightCallCount { get; private set; }
    public int RefreshResultViewCallCount { get; private set; }
    public int LastHighlightedInstructionIndex { get; private set; } = -1;

    public List<int> HighlightedInstructionIndices { get; } = new List<int>();

    public TestGame CreatedGame => createdGame_;

    public void AddTestInstruction()
    {
        AddInstructionToCurrentProgram(new TestInstructionDefinition());
    }

    public void SetExecutionDelay(float seconds)
    {
        SetPrivateFieldInHierarchy(this, "stepDelaySeconds_", seconds);
    }

    protected override void InitializeView()
    {
        InitializeViewCallCount++;
    }

    protected override TestGame CreateGame()
    {
        createdGame_ = new TestGame();
        return createdGame_;
    }

    protected override void RefreshViewImmediate()
    {
        RefreshImmediateCallCount++;
    }

    protected override IEnumerator RefreshViewAnimated()
    {
        RefreshAnimatedCallCount++;
        yield break;
    }

    protected override void HighlightInstruction(int instructionIndex)
    {
        base.HighlightInstruction(instructionIndex);

        LastHighlightedInstructionIndex = instructionIndex;

        if (instructionIndex >= 0)
        {
            HighlightedInstructionIndices.Add(instructionIndex);
        }
    }

    protected override void ClearInstructionHighlight()
    {
        base.ClearInstructionHighlight();

        ClearHighlightCallCount++;
        LastHighlightedInstructionIndex = -1;
    }

    protected override void RefreshResultView()
    {
        RefreshResultViewCallCount++;
    }

    private static void SetPrivateFieldInHierarchy(
        object target,
        string fieldName,
        object value)
    {
        Type currentType = target.GetType();

        while (currentType != null)
        {
            FieldInfo field = currentType.GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }

            currentType = currentType.BaseType;
        }

        throw new InvalidOperationException(
            $"Field '{fieldName}' was not found in the type hierarchy.");
    }
}