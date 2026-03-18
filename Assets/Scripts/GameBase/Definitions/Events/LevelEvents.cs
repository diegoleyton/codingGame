using Flowbit.Utilities.Core.Events;

/// <summary>
/// Event called when a level has been loaded
/// </summary>
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

/// <summary>
/// Event called when a level has been successfully completed
/// </summary>
public sealed class LevelCompletedEvent : IEvent
{
    public LevelCompletedEvent(string levelId)
    {
        LevelId = levelId;
    }

    public string LevelId { get; }
}

/// <summary>
/// Event called when a level has been failed
/// </summary>
public sealed class LevelFailedEvent : IEvent
{
    public LevelFailedEvent(string levelId)
    {
        LevelId = levelId;
    }

    public string LevelId { get; }
}