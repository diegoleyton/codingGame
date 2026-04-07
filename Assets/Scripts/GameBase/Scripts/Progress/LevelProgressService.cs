using Flowbit.GameBase.Definitions;
using Flowbit.Utilities.Core.Events;
using Flowbit.Utilities.Storage;

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

            eventDispatcher_?.Subscribe<LevelUnlockedEvent>(OnLevelUnlocked);
        }

        public int GetHighestUnlockedLevelIndex()
        {
            return highestUnlockedLevelIndex_;
        }

        public bool IsLevelUnlocked(int levelIndex)
        {
            return levelIndex >= 0 && levelIndex <= highestUnlockedLevelIndex_;
        }

        private async void OnLevelUnlocked(LevelUnlockedEvent levelUnlockedEvent)
        {
            if (levelUnlockedEvent.LevelIndex <= highestUnlockedLevelIndex_)
            {
                return;
            }

            highestUnlockedLevelIndex_ = levelUnlockedEvent.LevelIndex;
            await dataStorage_.SaveAsync(StorageKey, new LevelProgressData
            {
                highestUnlockedLevelIndex = highestUnlockedLevelIndex_
            });
        }
    }
}
