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
    /// Event called when a level completed popup has been open
    /// </summary>
    public sealed class OnLevelCompletedPopupEvent : IEvent { }
}
