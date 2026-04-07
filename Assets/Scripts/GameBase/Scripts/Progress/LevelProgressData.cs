using System;

namespace Flowbit.GameBase.Progress
{
    /// <summary>
    /// Represents the persisted level unlock state for the game.
    /// </summary>
    [Serializable]
    public sealed class LevelProgressData
    {
        /// <summary>
        /// Gets or sets the highest unlocked level index.
        /// Level index 0 means only the first level is unlocked.
        /// </summary>
        public int highestUnlockedLevelIndex;
    }
}
