using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Flowbit.Engine;
using Flowbit.Engine.Instructions;
using Flowbit.EngineController;

/// <summary>
/// Simple fake game used to test the controller.
/// </summary>
public sealed class GameMock : IGame
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
/// Test instruction ids used by the controller play mode tests.
/// </summary>
public enum TestInstructionType
{
    Primitive = 1
}

/// <summary>
/// Simple primitive instruction used by tests.
/// </summary>
public sealed class InstructionDefinitionMock
    : GameInstructionDefinitionBase<GameMock, TestInstructionType>
{
    public override TestInstructionType GetInstructionId()
    {
        return TestInstructionType.Primitive;
    }

    public override string GetDisplayName()
    {
        return "Test Instruction";
    }

    public override bool IsPrimitive()
    {
        return true;
    }

    protected override void ExecuteTyped(
        GameMock game,
        InstructionInstance<TestInstructionType> instructionInstance)
    {
        game.RegisterExecution();
    }
}

/// <summary>
/// Factory used by tests to create instruction definitions from ids.
/// </summary>
public sealed class InstructionFactoryMock
    : IInstructionFactory<GameMock, TestInstructionType>
{
    public GameInstructionDefinitionBase<GameMock, TestInstructionType> CreateInstruction(
        TestInstructionType instructionType)
    {
        return instructionType switch
        {
            TestInstructionType.Primitive => new InstructionDefinitionMock(),
            _ => throw new NotSupportedException(
                $"Unsupported test instruction type '{instructionType}'.")
        };
    }
}

/// <summary>
/// Available-instructions resolver used by tests.
/// </summary>
public sealed class AvailableInstructionsResolverMock
    : AvailableInstructionsResolverBase<TestInstructionType>
{
    private static readonly IReadOnlyList<TestInstructionType> instructions_ =
        new List<TestInstructionType>
        {
            TestInstructionType.Primitive
        };

    public override IReadOnlyList<TestInstructionType> Resolve(int levelId)
    {
        return instructions_;
    }
}

/// <summary>
/// Concrete controller used to test GameControllerBase behavior.
/// </summary>
public sealed class GameControllerMock : GameControllerBase<GameMock, TestInstructionType>
{
    private readonly IInstructionFactory<GameMock, TestInstructionType> instructionFactory_ =
        new InstructionFactoryMock();

    private readonly IAvailableInstructionsResolver<TestInstructionType>
        availableInstructionsResolver_ = new AvailableInstructionsResolverMock();

    private GameMock createdGame_;

    public int InitializeViewCallCount { get; private set; }
    public int RefreshImmediateCallCount { get; private set; }
    public int RefreshAnimatedCallCount { get; private set; }
    public int ClearHighlightCallCount { get; private set; }
    public int RefreshResultViewCallCount { get; private set; }

    public int LastHighlightedInstructionIndex { get; private set; } = -1;

    public List<int> HighlightedInstructionIndices { get; } = new List<int>();

    public GameMock CreatedGame => createdGame_;

    protected override IInstructionFactory<GameMock, TestInstructionType> InstructionFactory =>
        instructionFactory_;

    protected override IAvailableInstructionsResolver<TestInstructionType>
        AvailableInstructionsResolver => availableInstructionsResolver_;

    public void AddTestInstruction()
    {
        AddInstructionToCurrentProgram(TestInstructionType.Primitive);
    }

    public void SetExecutionDelay(float seconds)
    {
        SetPrivateFieldInHierarchy(this, "stepDelaySeconds_", seconds);
    }

    protected override void InitializeView()
    {
        InitializeViewCallCount++;
    }

    protected override GameMock CreateGame()
    {
        createdGame_ = new GameMock();
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
        base.RefreshResultView();
        RefreshResultViewCallCount++;
    }

    protected override void ApplyExecutedStepImmediate()
    {
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