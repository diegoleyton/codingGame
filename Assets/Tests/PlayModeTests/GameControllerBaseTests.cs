using System.Collections;
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
        TestGameController controller = gameObject.AddComponent<TestGameController>();

        yield return null;

        Assert.IsNotNull(controller.CreatedGame);
        Assert.AreEqual(1, controller.InitializeViewCallCount);
        Assert.AreEqual(1, controller.RefreshImmediateCallCount);
        Assert.AreEqual(1, controller.ClearHighlightCallCount);
        Assert.AreEqual(1, controller.RefreshResultViewCallCount);

        UnityEngine.Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator Step_WithInstruction_ExecutesInstructionAndHighlightsIt()
    {
        GameObject gameObject = new GameObject("TestController");
        TestGameController controller = gameObject.AddComponent<TestGameController>();

        yield return null;

        controller.AddTestInstruction();
        controller.Step();

        yield return null;

        Assert.AreEqual(1, controller.CreatedGame.TotalExecutedInstructions);
        Assert.AreEqual(1, controller.RefreshAnimatedCallCount);
        Assert.AreEqual(0, controller.LastHighlightedInstructionIndex);

        UnityEngine.Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator Run_WithMultipleInstructions_ExecutesAllInstructionsAndHighlightsEachStep()
    {
        GameObject gameObject = new GameObject("TestController");
        TestGameController controller = gameObject.AddComponent<TestGameController>();

        yield return null;

        controller.SetExecutionDelay(0f);

        controller.AddTestInstruction();
        controller.AddTestInstruction();
        controller.AddTestInstruction();

        controller.Run();

        yield return new WaitUntil(() =>
            controller.CreatedGame.TotalExecutedInstructions == 3);

        Assert.AreEqual(3, controller.CreatedGame.TotalExecutedInstructions);
        Assert.AreEqual(3, controller.RefreshAnimatedCallCount);

        CollectionAssert.AreEqual(
            new[] { 0, 1, 2 },
            controller.HighlightedInstructionIndices);

        UnityEngine.Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator ResetGame_ClearsHighlight_ResetsGame_AndShowsNoFurtherExecution()
    {
        GameObject gameObject = new GameObject("TestController");
        TestGameController controller = gameObject.AddComponent<TestGameController>();

        yield return null;

        controller.AddTestInstruction();
        controller.Step();

        yield return null;

        int clearHighlightCallCountBeforeReset = controller.ClearHighlightCallCount;
        int resetCallCountBeforeReset = controller.CreatedGame.ResetCallCount;

        controller.ResetGame();

        Assert.AreEqual(resetCallCountBeforeReset + 1, controller.CreatedGame.ResetCallCount);
        Assert.AreEqual(clearHighlightCallCountBeforeReset + 1, controller.ClearHighlightCallCount);
        Assert.AreEqual(-1, controller.LastHighlightedInstructionIndex);

        UnityEngine.Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator AfterReset_AddingMoreInstructions_UsesUpdatedProgram()
    {
        GameObject gameObject = new GameObject("TestController");
        TestGameController controller = gameObject.AddComponent<TestGameController>();

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

        UnityEngine.Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator CleanProgram_RemovesInstructions_AndNextStepDoesNothingUntilNewInstructionIsAdded()
    {
        GameObject gameObject = new GameObject("TestController");
        TestGameController controller = gameObject.AddComponent<TestGameController>();

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

        UnityEngine.Object.Destroy(gameObject);
    }
}