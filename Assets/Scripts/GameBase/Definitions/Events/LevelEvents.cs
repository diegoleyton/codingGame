using Flowbit.Utilities.Events;

public sealed class LevelLoadedEvent : IEvent
{
    public LevelLoadedEvent(string levelId, int levelIndex)
    {
        LevelId = levelId;
        LevelIndex = levelIndex;
    }

    public string LevelId { get; }
    public int LevelIndex { get; }
}

public sealed class LevelCompletedEvent : IEvent
{
    public LevelCompletedEvent(string levelId)
    {
        LevelId = levelId;
    }

    public string LevelId { get; }
}

public sealed class LevelFailedEvent : IEvent
{
    public LevelFailedEvent(string levelId)
    {
        LevelId = levelId;
    }

    public string LevelId { get; }
}