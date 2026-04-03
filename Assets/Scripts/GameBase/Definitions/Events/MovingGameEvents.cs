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
}