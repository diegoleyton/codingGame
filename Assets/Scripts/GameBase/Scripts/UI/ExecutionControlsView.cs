using UnityEngine;
using Flowbit.EngineController;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Displays the current execution state for the play/pause and stop controls.
    /// </summary>
    public sealed class ExecutionControlsView : ExecutionControlsViewBase
    {
        [SerializeField]
        private GameObject playIcon_;

        [SerializeField]
        private GameObject pauseIcon_;

        [SerializeField]
        private GameObject stopButtonRoot_;

        /// <summary>
        /// Updates the control visuals based on whether the game is running.
        /// </summary>
        public override void SetIsRunning(bool isRunning)
        {
            if (playIcon_ != null)
            {
                playIcon_.SetActive(!isRunning);
            }

            if (pauseIcon_ != null)
            {
                pauseIcon_.SetActive(isRunning);
            }
        }

        /// <summary>
        /// Updates the visibility of the stop control.
        /// </summary>
        public override void SetStopButtonVisible(bool visible)
        {
            if (stopButtonRoot_ != null)
            {
                stopButtonRoot_.SetActive(visible);
            }
        }
    }
}