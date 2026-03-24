using System;
using Flowbit.Utilities.Navigation;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Navigation parameters used when opening the moving game scene.
    /// </summary>
    [Serializable]
    public sealed class MovingGameNavigationParams : NavigationParams
    {
        /// <summary>
        /// Creates a new moving game navigation params instance.
        /// </summary>
        public MovingGameNavigationParams(int levelIndex)
        {
            if (levelIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(levelIndex),
                    "Level index must be greater than or equal to zero.");
            }

            LevelIndex = levelIndex;
        }

        /// <summary>
        /// Gets the requested level index.
        /// </summary>
        public int LevelIndex { get; }
    }
}