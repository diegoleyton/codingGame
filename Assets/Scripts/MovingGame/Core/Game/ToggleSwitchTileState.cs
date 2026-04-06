using System;

namespace Flowbit.MovingGame.Core
{
    /// <summary>
    /// Represents a toggle switch tile in the grid.
    /// 
    /// When the player steps on this tile, it toggles all
    /// <see cref="ToggleBlockedObstacleState"/> instances that share the same groupId.
    /// 
    /// The switch itself does not maintain state and can be triggered multiple times.
    /// </summary>
    public sealed class ToggleSwitchTileState
    {
        /// <summary>
        /// Gets the position of this switch in the grid.
        /// </summary>
        public GridPosition Position { get; }

        /// <summary>
        /// Gets the group identifier that this switch controls.
        /// All toggleable obstacles with the same groupId will be toggled when activated.
        /// </summary>
        public int GroupId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleSwitchTileState"/> class.
        /// </summary>
        /// <param name="position">The grid position of the switch.</param>
        /// <param name="groupId">The group identifier to toggle.</param>
        public ToggleSwitchTileState(GridPosition position, int groupId)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }
            Position = position;
            GroupId = groupId;
        }

        /// <summary>
        /// Creates a copy of this switch state.
        /// </summary>
        /// <returns>A new instance with the same values.</returns>
        public ToggleSwitchTileState Clone()
        {
            return new ToggleSwitchTileState(Position, GroupId);
        }
    }
}