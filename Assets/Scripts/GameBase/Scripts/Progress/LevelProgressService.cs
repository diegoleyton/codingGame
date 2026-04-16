using Flowbit.GameBase.Definitions;
using Flowbit.Utilities.Core.Events;
using Flowbit.Utilities.Storage;
using System.Collections.Generic;

namespace Flowbit.GameBase.Progress
{
    /// <summary>
    /// Keeps level unlock progress in memory and persists it through a repository.
    /// </summary>
    public sealed class LevelProgressService : ILevelProgressService
    {
        private const string StorageKey = "moving_game_level_progress";

        private readonly IDataStorage dataStorage_;
        private readonly EventDispatcher eventDispatcher_;
        private int highestUnlockedLevelIndex_;
        private readonly Dictionary<int, int> bestStarsByLevelIndex_ = new();

        public LevelProgressService(EventDispatcher eventDispatcher, IDataStorage dataStorage)
        {
            eventDispatcher_ = eventDispatcher;
            dataStorage_ = dataStorage;

            DataLoadResult<LevelProgressData> loadResult = dataStorage_
                .LoadAsync<LevelProgressData>(StorageKey)
                .GetAwaiter()
                .GetResult();

            LevelProgressData progressData = loadResult.Data ?? new LevelProgressData();

            highestUnlockedLevelIndex_ = progressData.highestUnlockedLevelIndex < 0
                ? 0
                : progressData.highestUnlockedLevelIndex;

            bestStarsByLevelIndex_.Clear();
            if (progressData.bestRankings != null)
            {
                for (int i = 0; i < progressData.bestRankings.Length; i++)
                {
                    LevelBestRankingData rankingData = progressData.bestRankings[i];
                    if (rankingData == null || rankingData.levelIndex < 0)
                    {
                        continue;
                    }

                    int starCount = rankingData.starCount < 0 ? 0 : rankingData.starCount;
                    if (bestStarsByLevelIndex_.TryGetValue(rankingData.levelIndex, out int currentBest))
                    {
                        bestStarsByLevelIndex_[rankingData.levelIndex] = starCount > currentBest
                            ? starCount
                            : currentBest;
                    }
                    else
                    {
                        bestStarsByLevelIndex_[rankingData.levelIndex] = starCount;
                    }
                }
            }

            eventDispatcher_?.Subscribe<LevelUnlockedEvent>(OnLevelUnlocked);
            eventDispatcher_?.Subscribe<LevelRankingRecordedEvent>(OnLevelRankingRecorded);
        }

        public int GetHighestUnlockedLevelIndex()
        {
            return highestUnlockedLevelIndex_;
        }

        public bool IsLevelUnlocked(int levelIndex)
        {
            return levelIndex >= 0 && levelIndex <= highestUnlockedLevelIndex_;
        }

        public int GetBestStarCount(int levelIndex)
        {
            if (levelIndex < 0)
            {
                return 0;
            }

            return bestStarsByLevelIndex_.TryGetValue(levelIndex, out int bestStarCount)
                ? bestStarCount
                : 0;
        }

        private async void OnLevelUnlocked(LevelUnlockedEvent levelUnlockedEvent)
        {
            if (levelUnlockedEvent.LevelIndex <= highestUnlockedLevelIndex_)
            {
                return;
            }

            highestUnlockedLevelIndex_ = levelUnlockedEvent.LevelIndex;
            await SaveProgressAsync();
        }

        private async void OnLevelRankingRecorded(LevelRankingRecordedEvent levelRankingRecordedEvent)
        {
            if (levelRankingRecordedEvent.LevelIndex < 0)
            {
                return;
            }

            int newStarCount = levelRankingRecordedEvent.StarCount < 0
                ? 0
                : levelRankingRecordedEvent.StarCount;

            if (bestStarsByLevelIndex_.TryGetValue(levelRankingRecordedEvent.LevelIndex, out int currentBest) &&
                newStarCount <= currentBest)
            {
                return;
            }

            bestStarsByLevelIndex_[levelRankingRecordedEvent.LevelIndex] = newStarCount;
            await SaveProgressAsync();
        }

        private async System.Threading.Tasks.Task SaveProgressAsync()
        {
            var bestRankings = new List<LevelBestRankingData>(bestStarsByLevelIndex_.Count);
            foreach (var pair in bestStarsByLevelIndex_)
            {
                bestRankings.Add(new LevelBestRankingData
                {
                    levelIndex = pair.Key,
                    starCount = pair.Value
                });
            }

            await dataStorage_.SaveAsync(StorageKey, new LevelProgressData
            {
                highestUnlockedLevelIndex = highestUnlockedLevelIndex_,
                bestRankings = bestRankings.ToArray()
            });
        }
    }
}
