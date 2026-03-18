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
        public List<MovingGameLevelData> levels;
    }
}