using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Play mode tests for GameControllerBase.
/// </summary>
public sealed class GameControllerBaseTests
{
    [UnityTest]
    public IEnumerator Start_InitializesGameAndRefreshesView()
    {
        GameObject gameObject = new GameObject("TestController");
        GameControllerMock controller = gameObject.AddComponent<GameControllerMock>();

        yield return null;

        Assert.IsNotNull(controller.CreatedGame);
        Assert.AreEqual(1, controller.InitializeViewCallCount);
        Assert.AreEqual(1, controller.RefreshImmediateCallCount);
        Assert.AreEqual(1, controller.RefreshResultViewCallCount);

        // The controller should start without any highlighted instruction.
        Assert.AreEqual(-1, controller.LastHighlightedInstructionIndex);

        Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator Step_WithInstruction_ExecutesInstruction()
    {
        GameObject gameObject = new GameObject("TestController");
        GameControllerMock controller = gameObject.AddComponent<GameControllerMock>();

        yield return null;

        controller.AddTestInstruction();
        controller.Step();

        yield return null;

        Assert.AreEqual(1, controller.CreatedGame.TotalExecutedInstructions);
        Assert.GreaterOrEqual(controller.RefreshAnimatedCallCount, 1);

        Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator Run_WithMultipleInstructions_ExecutesAllInstructions()
    {
        GameObject gameObject = new GameObject("TestController");
        GameControllerMock controller = gameObject.AddComponent<GameControllerMock>();

        yield return null;

        controller.SetExecutionDelay(0f);
        controller.AddTestInstruction();
        controller.AddTestInstruction();
        controller.AddTestInstruction();

        controller.Run();

        yield return new WaitUntil(() => controller.CreatedGame.TotalExecutedInstructions == 3);
        yield return null;

        Assert.AreEqual(3, controller.CreatedGame.TotalExecutedInstructions);
        Assert.GreaterOrEqual(controller.RefreshAnimatedCallCount, 3);

        Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator ResetGame_ClearsHighlight_ResetsGame_AndShowsNoFurtherExecution()
    {
        GameObject gameObject = new GameObject("TestController");
        GameControllerMock controller = gameObject.AddComponent<GameControllerMock>();

        yield return null;

        controller.AddTestInstruction();
        controller.Step();

        yield return null;

        int resetCallCountBeforeReset = controller.CreatedGame.ResetCallCount;
        int executedInstructionCountBeforeReset = controller.CreatedGame.TotalExecutedInstructions;

        controller.ResetGame();

        yield return null;

        Assert.AreEqual(resetCallCountBeforeReset + 1, controller.CreatedGame.ResetCallCount);

        // Reset itself should not execute any new instruction.
        Assert.AreEqual(executedInstructionCountBeforeReset, controller.CreatedGame.TotalExecutedInstructions);

        Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator AfterReset_AddingMoreInstructions_UsesUpdatedProgram()
    {
        GameObject gameObject = new GameObject("TestController");
        GameControllerMock controller = gameObject.AddComponent<GameControllerMock>();

        yield return null;

        controller.AddTestInstruction();
        controller.Step();

        yield return null;

        Assert.AreEqual(1, controller.CreatedGame.TotalExecutedInstructions);

        controller.ResetGame();
        controller.AddTestInstruction();
        controller.AddTestInstruction();

        controller.Step();
        yield return null;

        controller.Step();
        yield return null;

        Assert.AreEqual(3, controller.CreatedGame.TotalExecutedInstructions);

        Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator CleanProgram_RemovesInstructions_AndNextStepDoesNothingUntilNewInstructionIsAdded()
    {
        GameObject gameObject = new GameObject("TestController");
        GameControllerMock controller = gameObject.AddComponent<GameControllerMock>();

        yield return null;

        controller.AddTestInstruction();
        controller.CleanProgram();

        controller.Step();
        yield return null;

        Assert.AreEqual(0, controller.CreatedGame.TotalExecutedInstructions);

        controller.AddTestInstruction();
        controller.Step();
        yield return null;

        Assert.AreEqual(1, controller.CreatedGame.TotalExecutedInstructions);

        Object.Destroy(gameObject);
    }
}