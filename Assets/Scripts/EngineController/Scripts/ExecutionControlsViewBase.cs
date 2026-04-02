using UnityEngine;
using UnityEngine.UI;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Displays the current execution state for the play/pause controls.
    /// </summary>
    public abstract class ExecutionControlsViewBase : MonoBehaviour
    {
        /// <summary>
        /// Updates the control visuals based on whether the game is running.
        /// </summary>
        public abstract void SetIsRunning(bool isRunning);
    }
}