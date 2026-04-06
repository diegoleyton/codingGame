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
        /// Jumps exactly two cells forward.
        /// The jump fails if either crossed cell is out of bounds or blocked.
        /// </summary>
        void JumpForward();

        /// <summary>
        /// Returns the positions of all remaining food items.
        /// </summary>
        IReadOnlyCollection<GridPosition> GetFoodPositions();

        /// <summary>
        /// Returns the positions of solid obstacles.
        /// This includes only the classic always-blocking obstacles.
        /// </summary>
        IReadOnlyCollection<GridPosition> GetBlockedPositions();

        /// <summary>
        /// Returns the positions of breakable obstacles.
        /// </summary>
        IReadOnlyCollection<GridPosition> GetBreakableBlockedPositions();

        /// <summary>
        /// Returns the positions of hole cells.
        /// Holes block walking, but can be crossed by jumping.
        /// </summary>
        IReadOnlyCollection<GridPosition> GetHolePositions();

        /// <summary>
        /// Returns the positions visited by the character.
        /// </summary>
        IReadOnlyCollection<GridPosition> GetVisitedPositions();

        /// <summary>
        /// Returns all toggleable blocked obstacles currently present in the level.
        /// Each obstacle keeps its own on/off state.
        /// </summary>
        IReadOnlyCollection<ToggleBlockedObstacleState> GetToggleBlockedObstacles();

        /// <summary>
        /// Returns all toggle switch tiles currently present in the level.
        /// </summary>
        IReadOnlyCollection<ToggleSwitchTileState> GetToggleSwitchTiles();

        /// <summary>
        /// Tries to get the toggleable blocked obstacle at the given position.
        /// </summary>
        /// <param name="position">The position to query.</param>
        /// <param name="state">
        /// When this method returns, contains the obstacle state if one exists at the position;
        /// otherwise, null.
        /// </param>
        /// <returns>True if a toggleable blocked obstacle exists at the given position; otherwise false.</returns>
        bool TryGetToggleBlockedObstacle(GridPosition position, out ToggleBlockedObstacleState state);

        /// <summary>
        /// Tries to get the toggle switch tile at the given position.
        /// </summary>
        /// <param name="position">The position to query.</param>
        /// <param name="state">
        /// When this method returns, contains the switch state if one exists at the position;
        /// otherwise, null.
        /// </param>
        /// <returns>True if a toggle switch tile exists at the given position; otherwise false.</returns>
        bool TryGetToggleSwitchTile(GridPosition position, out ToggleSwitchTileState state);

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
