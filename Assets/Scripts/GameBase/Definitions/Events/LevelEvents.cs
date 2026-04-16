using Flowbit.Utilities.Core.Events;

namespace Flowbit.GameBase.Definitions
{
    /// <summary>
    /// Event called when a level has been loaded
    /// </summary>
    public sealed class LevelLoadedEvent : IEvent
    {
        public LevelLoadedEvent(int levelId, int levelIndex)
        {
            LevelId = levelId;
            LevelIndex = levelIndex;
        }

        public int LevelId { get; }
        public int LevelIndex { get; }
    }

    /// <summary>
    /// Event called when a level has been successfully completed
    /// </summary>
    public sealed class LevelCompletedEvent : IEvent
    {
        public LevelCompletedEvent(int levelId)
        {
            LevelId = levelId;
        }

        public int LevelId { get; }
    }

    /// <summary>
    /// Event called when a level has been failed
    /// </summary>
    public sealed class LevelFailedEvent : IEvent
    {
        public LevelFailedEvent(int levelId)
        {
            LevelId = levelId;
        }

        public int LevelId { get; }
    }

    /// <summary>
    /// Event called when a new level becomes unlocked.
    /// </summary>
    public sealed class LevelUnlockedEvent : IEvent
    {
        public LevelUnlockedEvent(int levelId, int levelIndex)
        {
            LevelId = levelId;
            LevelIndex = levelIndex;
        }

        public int LevelId { get; }
        public int LevelIndex { get; }
    }

    /// <summary>
    /// Event called when a level records a ranking result that should be persisted.
    /// </summary>
    public sealed class LevelRankingRecordedEvent : IEvent
    {
        public LevelRankingRecordedEvent(int levelId, int levelIndex, int starCount, int maxStars)
        {
            LevelId = levelId;
            LevelIndex = levelIndex;
            StarCount = starCount;
            MaxStars = maxStars;
        }

        public int LevelId { get; }
        public int LevelIndex { get; }
        public int StarCount { get; }
        public int MaxStars { get; }
    }

    /// <summary>
    /// Event called when the star fill animation starts.
    /// </summary>
    public sealed class OnStarFillStarted : IEvent { }

    /// <summary>
    /// Event called when the star fill animation finishes.
    /// </summary>
    public sealed class OnStarFillCompleted : IEvent { }

    /// <summary>
    /// Event called when a level completed popup has been open
    /// </summary>
    public sealed class OnLevelCompletedPopupEvent : IEvent { }
}
