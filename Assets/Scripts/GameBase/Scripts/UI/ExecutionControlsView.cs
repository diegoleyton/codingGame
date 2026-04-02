using UnityEngine;
using UnityEngine.UI;
using Flowbit.EngineController;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Displays the current execution state for the play/pause controls.
    /// </summary>
    public sealed class ExecutionControlsView : ExecutionControlsViewBase
    {
        [SerializeField]
        private Image playPauseIcon_;

        [SerializeField]
        private Sprite playSprite_;

        [SerializeField]
        private Sprite pauseSprite_;

        /// <summary>
        /// Updates the control visuals based on whether the game is running.
        /// </summary>
        public override void SetIsRunning(bool isRunning)
        {
            if (playPauseIcon_ == null)
            {
                return;
            }

            playPauseIcon_.sprite = isRunning ? pauseSprite_ : playSprite_;
        }
    }
}