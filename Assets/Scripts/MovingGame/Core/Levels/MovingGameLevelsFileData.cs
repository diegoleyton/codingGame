using System;
using System.Collections.Generic;

namespace Flowbit.MovingGame.Core.Levels
{
    /// <summary>
    /// Represents the root JSON object for a moving game levels file.
    /// </summary>
    [Serializable]
    public sealed class MovingGameLevelsFileData
    {
        public string gameType;
        public int version;
        public MovingGameRankingMetadataData rankingMetadata;
        public List<MovingGameLevelData> levels;
    }

    /// <summary>
    /// Stores ranking tuning metadata shared by the whole campaign.
    /// </summary>
    [Serializable]
    public sealed class MovingGameRankingMetadataData
    {
        public int maxStars = 4;
        public float baseTimeConstantSeconds = 2f;
        public float thinkTimePerDifficultySeconds = 2f;
        public float instructionTimePerStepSeconds = 1.75f;
        public float penaltyWindowBaseSeconds = 2.5f;
        public float penaltyWindowPerDifficultySeconds = 0.75f;
    }
}
