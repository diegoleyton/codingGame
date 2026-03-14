using CodingGame.Runtime.Core;
using System.Collections.Generic;

namespace CodingGame.Runtime.Games.Moving
{
    /// <summary>
    /// Represents a game where an agent can move and rotate.
    /// </summary>
    public interface IMovingGame : IGame
    {
        /// <summary>
        /// Moves the agent forward by the given number of steps.
        /// </summary>
        void MoveForward(int steps);

        /// <summary>
        /// Rotates the agent to the left.
        /// </summary>
        void RotateLeft();

        /// <summary>
        /// Rotates the agent to the right.
        /// </summary>
        void RotateRight();
    }
}