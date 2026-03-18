using System.Collections.Generic;
using Flowbit.Engine;

/// <summary>
/// A generic game test double used by engine unit tests.
/// </summary>
internal sealed class GameMock : IGame
{
    private readonly List<string> log_;
    private bool hasWon_;
    private bool hasFailed_;

    /// <summary>
    /// Creates a new game mock.
    /// </summary>
    public GameMock()
    {
        log_ = new List<string>();
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
    /// Appends a log entry.
    /// </summary>
    public void Log(string entry)
    {
        log_.Add(entry);
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
}