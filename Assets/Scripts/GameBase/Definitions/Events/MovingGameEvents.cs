using Flowbit.Utilities.Core.Events;

namespace Flowbit.GameBase.Definitions
{
    /// <summary>
    /// Event called when a character is moving
    /// </summary>
    public sealed class OnMovingGameMove : IEvent { }

    /// <summary>
    /// Event called when a character is rotating
    /// </summary>
    public sealed class OnMovingGameRotate : IEvent { }

    /// <summary>
    /// Event called when a character jumps.
    /// </summary>
    public sealed class OnMovingGameJump : IEvent { }

    /// <summary>
    /// Event called when an obstacle is broken
    /// </summary>
    public sealed class OnMovingGameBreak : IEvent { }

    /// <summary>
    /// Event called when a character attacks
    /// </summary>
    public sealed class OnMovingGameAttack : IEvent { }

    /// <summary>
    /// Event called when a character reaches a goal
    /// </summary>
    public sealed class OnMovingGameGoalReached : IEvent { }

    /// <summary>
    /// Event sent when the player steps on a toggle switch tile.
    /// </summary>
    public readonly struct OnMovingGameSwitch : IEvent
    {
        /// <summary>
        /// Gets the group id associated with the switch.
        /// </summary>
        public int GroupId { get; }

        public OnMovingGameSwitch(int groupId)
        {
            GroupId = groupId;
        }
    }
}
