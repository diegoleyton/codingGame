using System;
using Flowbit.MovingGame.Core.Levels;
using UnityEngine;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Loads moving game levels from a JSON text asset.
    /// </summary>
    public sealed class MovingGameLevelsLibrary : MonoBehaviour
    {
        [SerializeField] private TextAsset levelsJson_;

        private MovingGameLevelsFileData levelsFileData_;

        /// <summary>
        /// Loads and parses the levels file.
        /// </summary>
        public void Load()
        {
            if (levelsJson_ == null)
            {
                throw new InvalidOperationException("Levels JSON asset is not assigned.");
            }

            levelsFileData_ =
                MovingGameLevelsParser.Parse(levelsJson_.text);
        }

        /// <summary>
        /// Returns the total number of levels.
        /// </summary>
        public int GetLevelCount()
        {
            EnsureLoaded();
            return levelsFileData_.levels.Count;
        }

        /// <summary>
        /// Returns the level data at the given index.
        /// </summary>
        public MovingGameLevelData GetLevelAt(int index)
        {
            EnsureLoaded();
            return levelsFileData_.levels[index];
        }

        private void EnsureLoaded()
        {
            if (levelsFileData_ == null)
            {
                throw new InvalidOperationException("Levels file has not been loaded yet.");
            }
        }
    }
}