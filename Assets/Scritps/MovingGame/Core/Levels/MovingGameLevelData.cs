using System;
using System.Collections.Generic;

namespace Flowbit.MovingGame.Core.Levels
{
    /// <summary>
    /// Represents one moving game level loaded from JSON.
    /// </summary>
    [Serializable]
    public sealed class MovingGameLevelData
    {
        public string id;
        public string name;
        public int difficulty;
        public string hint;
        public int width;
        public int height;
        public PositionData startPosition;
        public string startDirection;
        public List<PositionData> foodPositions;
        public List<PositionData> blockedPositions;
        public List<PositionData> breakableBlockedPositions;
    }

    /// <summary>
    /// Represents a 2D grid position in serialized level data.
    /// </summary>
    [Serializable]
    public sealed class PositionData
    {
        public int x;
        public int y;
    }
}