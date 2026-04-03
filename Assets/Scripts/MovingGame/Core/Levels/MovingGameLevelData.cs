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
        public int id;
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

        public Dificulty GetDificulty()
        {
            if (difficulty <= 2)
            {
                return Dificulty.Easy;
            }
            if (difficulty <= 4)
            {
                return Dificulty.Normal;
            }
            return Dificulty.Hard;
        }
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

    /// <summary>
    /// Represnts the level of dificulty.
    /// </summary>
    public enum Dificulty
    {
        Easy,
        Normal,
        Hard
    }
}