using System;

namespace Flowbit.MovingGame.Core
{
    /// <summary>
    /// Represents a toggleable blocked obstacle in the grid.
    /// Each obstacle belongs to a group (groupId) and maintains its own on/off state.
    /// 
    /// When the obstacle is "on", it blocks movement.
    /// When the obstacle is "off", the cell is passable, but the obstacle still exists
    /// and can be toggled again by a corresponding switch.
    /// </summary>
    public sealed class ToggleBlockedObstacleState
    {
        /// <summary>
        /// Gets the position of this obstacle in the grid.
        /// </summary>
        public GridPosition Position { get; }

        /// <summary>
        /// Gets the group identifier used to toggle this obstacle.
        /// Obstacles with the same groupId are toggled together by switches.
        /// </summary>
        public int GroupId { get; }

        /// <summary>
        /// Gets a value indicating whether this obstacle is currently active (blocking).
        /// </summary>
        public bool IsOn { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleBlockedObstacleState"/> class.
        /// </summary>
        /// <param name="position">The grid position of the obstacle.</param>
        /// <param name="groupId">The group identifier for toggling.</param>
        /// <param name="isOn">The initial active state of the obstacle.</param>
        public ToggleBlockedObstacleState(GridPosition position, int groupId, bool isOn)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }
            Position = position;
            GroupId = groupId;
            IsOn = isOn;
        }

        /// <summary>
        /// Toggles the state of this obstacle (on becomes off, off becomes on).
        /// </summary>
        public void Toggle()
        {
            IsOn = !IsOn;
        }

        /// <summary>
        /// Sets the state of this obstacle explicitly.
        /// </summary>
        /// <param name="isOn">The new state.</param>
        public void SetState(bool isOn)
        {
            IsOn = isOn;
        }

        /// <summary>
        /// Creates a copy of this obstacle state.
        /// </summary>
        /// <returns>A new instance with the same values.</returns>
        public ToggleBlockedObstacleState Clone()
        {
            return new ToggleBlockedObstacleState(Position, GroupId, IsOn);
        }
    }
}