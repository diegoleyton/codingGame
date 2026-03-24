using System;
using Flowbit.Utilities.Navigation;
using Flowbit.MovingGame.Core.Levels;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Navigation parameters used when opening the level selection popup.
    /// </summary>
    [Serializable]
    public sealed class MovingGameLevelSelectionPopupParams : NavigationParams
    {
        /// <summary>
        /// Creates a new popup params instance.
        /// </summary>
        public MovingGameLevelSelectionPopupParams(
            MovingGameLevelData movingGameLevelData,
            int index,
            Action onContinue)
        {
            MovingGameLevelData = movingGameLevelData;
            Index = index;
            OnContinue = onContinue;
        }

        /// <summary>
        /// Gets the level data.
        /// </summary>
        public MovingGameLevelData MovingGameLevelData { get; }

        /// <summary>
        /// Gets the level index.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the action executed when continue is pressed.
        /// </summary>
        public Action OnContinue { get; }
    }
}