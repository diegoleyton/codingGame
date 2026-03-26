using Flowbit.Engine;
using System.Collections.Generic;

namespace Flowbit.MovingGame.Core
{
    /// <summary>
    /// Represents a game where an agent can move, rotate, and interact with obstacles.
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

        /// <summary>
        /// Breaks the breakable obstacle in the cell directly in front of the agent, if one exists.
        /// </summary>
        void BreakForward();

        /// <summary>
        /// Returns the positions of all remaining food items.
        /// </summary>
        IReadOnlyCollection<GridPosition> GetFoodPositions();

        /// <summary>
        /// Returns the positions of solid obstacles.
        /// </summary>
        IReadOnlyCollection<GridPosition> GetBlockedPositions();

        /// <summary>
        /// Returns the positions of breakable obstacles.
        /// </summary>
        IReadOnlyCollection<GridPosition> GetBreakableBlockedPositions();

        /// <summary>
        /// Returns the positions visited by the character.
        /// </summary>
        IReadOnlyCollection<GridPosition> GetVisitedPositions();

        /// <summary>
        /// Returns whether there is a step after-process pending.
        /// </summary>
        bool HasStepAfterProcess();

        /// <summary>
        /// Returns the current step after-process.
        /// </summary>
        StepAfterProcess GetStepAfterProcess();

        /// <summary>
        /// Finalizes the current step after-process.
        /// </summary>
        void FinalizeStepAfterProcess();

        /// <summary>
        /// Clears the current step after-process.
        /// </summary>
        void ClearStepAfterProcess();
    }
}