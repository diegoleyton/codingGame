using UnityEngine;

namespace Flowbit.EngineController
{
    /// <summary>
    /// Base view for execution controls such as play, pause, and stop.
    /// </summary>
    public abstract class ExecutionControlsViewBase : MonoBehaviour
    {
        /// <summary>
        /// Updates the visual state of the play/pause control.
        /// </summary>
        public abstract void SetIsRunning(bool isRunning);

        /// <summary>
        /// Updates the visibility of the stop control.
        /// </summary>
        public abstract void SetStopButtonVisible(bool visible);
    }
}