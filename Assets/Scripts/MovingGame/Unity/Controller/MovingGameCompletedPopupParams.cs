using System;
using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Navigation parameters used when opening the level completed popup.
    /// </summary>
    [Serializable]
    public sealed class MovingGameCompletedPopupParams : NavigationParams
    {
        /// <summary>
        /// Creates a new popup params instance.
        /// </summary>
        public MovingGameCompletedPopupParams(
            string nextLevelTitle,
            bool hasNextLevel,
            Action onContinue,
            Action onRetry,
            Action onClose)
        {
            NextLevelTitle = nextLevelTitle;
            HasNextLevel = hasNextLevel;
            OnContinue = onContinue;
            OnRetry = onRetry;
            OnClose = onClose;
        }

        /// <summary>
        /// Gets the title of the next level.
        /// </summary>
        public string NextLevelTitle { get; }

        /// <summary>
        /// Gets whether there is a next level available.
        /// </summary>
        public bool HasNextLevel { get; }

        /// <summary>
        /// Gets the action executed when continue is pressed.
        /// </summary>
        public Action OnContinue { get; }

        /// <summary>
        /// Gets the action executed when retry is pressed.
        /// </summary>
        public Action OnRetry { get; }

        /// <summary>
        /// Gets the action executed when close is pressed.
        /// </summary>
        public Action OnClose { get; }
    }
}