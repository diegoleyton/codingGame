using System.Collections.Generic;
using CodingGame.Runtime.Games.Moving;
using CodingGame.Runtime.Core;
using CodingGame.Runtime.Instructions;

/// <summary>
/// A test double for a moving game used by unit tests.
/// </summary>
internal sealed class FakeMovingGame : IMovingGame
{
    private readonly List<string> log_;
    private readonly List<IInstructionDefinition> availableInstructionDefinitions_;
    private bool hasWon_;
    private bool hasFailed_;

    /// <summary>
    /// Creates a new fake moving game.
    /// </summary>
    public FakeMovingGame()
    {
        log_ = new List<string>();
        availableInstructionDefinitions_ = new List<IInstructionDefinition>
        {
            new MoveForwardInstructionDefinition(),
            new RotateLeftInstructionDefinition(),
            new RotateRightInstructionDefinition(),
            new SequenceInstructionDefinition(),
            new RepeatInstructionDefinition()
        };

        hasWon_ = false;
        hasFailed_ = false;
    }

    /// <summary>
    /// Returns the execution log.
    /// </summary>
    public IReadOnlyList<string> GetLog()
    {
        return log_;
    }

    /// <summary>
    /// Sets whether the game is in a won state.
    /// </summary>
    public void SetWon(bool value)
    {
        hasWon_ = value;
    }

    /// <summary>
    /// Sets whether the game is in a failed state.
    /// </summary>
    public void SetFailed(bool value)
    {
        hasFailed_ = value;
    }

    /// <summary>
    /// Moves the agent forward by the given number of steps.
    /// </summary>
    public void MoveForward(int steps)
    {
        log_.Add($"Move({steps})");
    }

    /// <summary>
    /// Rotates the agent to the left.
    /// </summary>
    public void RotateLeft()
    {
        log_.Add("RotateLeft");
    }

    /// <summary>
    /// Rotates the agent to the right.
    /// </summary>
    public void RotateRight()
    {
        log_.Add("RotateRight");
    }

    /// <summary>
    /// Returns whether the game has been completed successfully.
    /// </summary>
    public bool HasWon()
    {
        return hasWon_;
    }

    /// <summary>
    /// Returns whether the game has reached a failed state.
    /// </summary>
    public bool HasFailed()
    {
        return hasFailed_;
    }

    /// <summary>
    /// Resets the game to its initial state.
    /// </summary>
    public void ResetGame()
    {
        log_.Clear();
        hasWon_ = false;
        hasFailed_ = false;
    }

    /// <summary>
    /// Returns the instruction definitions available for this game.
    /// </summary>
    public IReadOnlyList<IInstructionDefinition> GetAvailableInstructionDefinitions()
    {
        return availableInstructionDefinitions_;
    }
}