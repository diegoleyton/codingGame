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

        /// <summary>
        /// Gets or sets the best star result stored for each level index.
        /// </summary>
        public LevelBestRankingData[] bestRankings;
    }

    /// <summary>
    /// Represents the best stored ranking for a single level.
    /// </summary>
    [Serializable]
    public sealed class LevelBestRankingData
    {
        public int levelIndex;
        public int starCount;
    }
}
