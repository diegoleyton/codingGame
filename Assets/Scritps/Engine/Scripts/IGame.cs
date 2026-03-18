using System.Collections.Generic;

namespace Flowbit.Engine
{
    /// <summary>
    /// Represents the minimal contract for any game that can be used by the instruction runner.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Returns whether the game has been completed successfully.
        /// </summary>
        bool HasWon();

        /// <summary>
        /// Returns whether the game has reached a failed state.
        /// </summary>
        bool HasFailed();

        /// <summary>
        /// Resets the game to its initial state.
        /// </summary>
        void ResetGame();
    }
}