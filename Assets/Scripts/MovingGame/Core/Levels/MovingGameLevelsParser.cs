using System;
using System.Collections.Generic;

namespace Flowbit.MovingGame.Core.Levels
{
    /// <summary>
    /// Parses moving game levels from JSON text.
    /// </summary>
    public static class MovingGameLevelsParser
    {
        /// <summary>
        /// Parses a levels JSON file.
        /// </summary>
        public static MovingGameLevelsFileData Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON cannot be null or empty.", nameof(json));
            }

            MovingGameLevelsFileData fileData =
                UnityEngine.JsonUtility.FromJson<MovingGameLevelsFileData>(json);

            if (fileData == null)
            {
                throw new InvalidOperationException("Failed to parse moving game levels JSON.");
            }

            if (fileData.levels == null)
            {
                throw new InvalidOperationException("Levels list is missing in the JSON file.");
            }

            Validate(fileData);
            return fileData;
        }

        private static void Validate(MovingGameLevelsFileData fileData)
        {
            for (int i = 0; i < fileData.levels.Count; i++)
            {
                MovingGameLevelData level = fileData.levels[i];

                if (level.id <= 0)
                {
                    throw new InvalidOperationException($"Level at index {i} has invalid id '{level.id}'.");
                }

                if (level.width <= 0 || level.height <= 0)
                {
                    throw new InvalidOperationException(
                        $"Level '{level.id}' has invalid dimensions.");
                }

                if (level.startPosition == null)
                {
                    throw new InvalidOperationException(
                        $"Level '{level.id}' is missing startPosition.");
                }

                if (string.IsNullOrWhiteSpace(level.startDirection))
                {
                    throw new InvalidOperationException(
                        $"Level '{level.id}' is missing startDirection.");
                }

                if (level.foodPositions == null)
                {
                    level.foodPositions = new List<PositionData>();
                }

                if (level.blockedPositions == null)
                {
                    level.blockedPositions = new List<PositionData>();
                }

                if (level.breakableBlockedPositions == null)
                {
                    level.breakableBlockedPositions = new List<PositionData>();
                }
            }
        }
    }
}