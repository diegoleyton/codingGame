using UnityEngine;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Displays the current status of the game.
    /// </summary>
    public abstract class GameStatusViewBase : MonoBehaviour
    {

        /// <summary>
        /// Sets the win state
        /// </summary>
        public abstract void Win();

        /// <summary>
        /// Sets the lose state
        /// </summary>
        public abstract void Lose();

        /// <summary>
        /// Sets the idle state
        /// </summary>
        public abstract void Idle();
    }
}