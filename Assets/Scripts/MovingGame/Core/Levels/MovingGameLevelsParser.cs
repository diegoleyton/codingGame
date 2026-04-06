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

            MovingGameLevelsFileData fileData = UnityEngine.JsonUtility.FromJson<MovingGameLevelsFileData>(json);
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
                    throw new InvalidOperationException($"Level '{level.id}' has invalid dimensions.");
                }

                if (level.startPosition == null)
                {
                    throw new InvalidOperationException($"Level '{level.id}' is missing startPosition.");
                }

                if (string.IsNullOrWhiteSpace(level.startDirection))
                {
                    throw new InvalidOperationException($"Level '{level.id}' is missing startDirection.");
                }

                level.foodPositions ??= new List<PositionData>();
                level.blockedPositions ??= new List<PositionData>();
                level.breakableBlockedPositions ??= new List<PositionData>();
                level.holePositions ??= new List<PositionData>();
                level.toggleBlockedTiles ??= new List<ToggleBlockedTileData>();
                level.toggleSwitchTiles ??= new List<ToggleSwitchTileData>();

                ValidatePositions(level, level.foodPositions, "foodPositions");
                ValidatePositions(level, level.blockedPositions, "blockedPositions");
                ValidatePositions(level, level.breakableBlockedPositions, "breakableBlockedPositions");
                ValidatePositions(level, level.holePositions, "holePositions");
                ValidateToggleData(level);
            }
        }

        private static void ValidatePositions(
            MovingGameLevelData level,
            List<PositionData> positions,
            string source)
        {
            foreach (PositionData position in positions)
            {
                if (position == null)
                {
                    throw new InvalidOperationException(
                        $"Level '{level.id}' contains a null entry in '{source}'.");
                }

                ValidatePosition(level, position.x, position.y, source);
            }
        }

        private static void ValidateToggleData(MovingGameLevelData level)
        {
            HashSet<int> groupIdsWithObstacles = new();

            foreach (ToggleBlockedTileData tile in level.toggleBlockedTiles)
            {
                ValidatePosition(level, tile.x, tile.y, "toggleBlockedTiles");

                if (tile.groupId <= 0)
                {
                    throw new InvalidOperationException(
                        $"Level '{level.id}' has toggle blocked tile with invalid groupId '{tile.groupId}'.");
                }

                groupIdsWithObstacles.Add(tile.groupId);
            }

            foreach (ToggleSwitchTileData tile in level.toggleSwitchTiles)
            {
                ValidatePosition(level, tile.x, tile.y, "toggleSwitchTiles");

                if (tile.groupId <= 0)
                {
                    throw new InvalidOperationException(
                        $"Level '{level.id}' has toggle switch tile with invalid groupId '{tile.groupId}'.");
                }

                if (!groupIdsWithObstacles.Contains(tile.groupId))
                {
                    throw new InvalidOperationException(
                        $"Level '{level.id}' has toggle switch group '{tile.groupId}' without matching toggle obstacles.");
                }
            }
        }

        private static void ValidatePosition(MovingGameLevelData level, int x, int y, string source)
        {
            if (x < 0 || x >= level.width || y < 0 || y >= level.height)
            {
                throw new InvalidOperationException(
                    $"Level '{level.id}' contains out-of-bounds position ({x}, {y}) in '{source}'.");
            }
        }
    }
}
