using System;
using Flowbit.EngineController;
using Flowbit.MovingGame.Core.Levels;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Moving game ranking implementation based on editable time.
    /// </summary>
    public sealed class MovingGameTimeRankingTracker : IGameRankingTracker
    {
        private readonly MovingGameLevelData levelData_;
        private readonly MovingGameRankingMetadataData rankingMetadata_;
        private readonly int maxStars_;

        private float elapsedSeconds_;

        public MovingGameTimeRankingTracker(
            MovingGameLevelData levelData,
            MovingGameRankingMetadataData rankingMetadata)
        {
            if (levelData == null)
            {
                throw new ArgumentNullException(nameof(levelData));
            }

            if (rankingMetadata == null)
            {
                throw new ArgumentNullException(nameof(rankingMetadata));
            }

            levelData_ = levelData;
            rankingMetadata_ = rankingMetadata;
            maxStars_ = Math.Max(1, rankingMetadata_.maxStars);
            Restart();
        }

        public void Restart()
        {
            elapsedSeconds_ = 0f;
        }

        public void SetElapsedSeconds(float elapsedSeconds)
        {
            elapsedSeconds_ = Math.Max(0f, elapsedSeconds);
        }

        public GameRankingResult GetCurrentResult()
        {
            float targetTimeSeconds = CalculateBaseTimeSeconds();
            float penaltyWindowSeconds = CalculatePenaltyWindowSeconds();

            int starCount = maxStars_;
            if (elapsedSeconds_ > targetTimeSeconds && penaltyWindowSeconds > 0f)
            {
                float overtimeSeconds = elapsedSeconds_ - targetTimeSeconds;
                int starsLost = (int)Math.Ceiling(overtimeSeconds / penaltyWindowSeconds);
                starCount = maxStars_ - starsLost;
                if (starCount < 1)
                {
                    starCount = 1;
                }
            }

            return new GameRankingResult(
                starCount: starCount,
                maxStars: maxStars_,
                summaryText: FormatTime(elapsedSeconds_));
        }

        private static string FormatTime(float elapsedSeconds)
        {
            int minutes = UnityEngine.Mathf.FloorToInt(elapsedSeconds / 60f);
            float seconds = elapsedSeconds - (minutes * 60f);
            return $"{minutes:00}:{seconds:00.0}";
        }

        private float CalculateBaseTimeSeconds()
        {
            int difficulty = Math.Max(1, levelData_.difficulty);
            int targetInstructionCount = Math.Max(1, levelData_.targetInstructionCount);

            float thinkTimeSeconds = difficulty * Math.Max(0f, rankingMetadata_.thinkTimePerDifficultySeconds);
            float instructionsTimeSeconds =
                targetInstructionCount * Math.Max(0.1f, rankingMetadata_.instructionTimePerStepSeconds);

            return Math.Max(0f, rankingMetadata_.baseTimeConstantSeconds) +
                   thinkTimeSeconds +
                   instructionsTimeSeconds;
        }

        private float CalculatePenaltyWindowSeconds()
        {
            int difficulty = Math.Max(1, levelData_.difficulty);
            float penaltyWindowSeconds =
                Math.Max(0f, rankingMetadata_.penaltyWindowBaseSeconds) +
                (difficulty * Math.Max(0f, rankingMetadata_.penaltyWindowPerDifficultySeconds));

            return Math.Max(0.1f, penaltyWindowSeconds);
        }
    }
}
