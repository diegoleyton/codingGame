using System.Collections.Generic;
using System.Linq;

using Flowbit.MovingGame.Core.Levels;

namespace Flowbit.MovingGame.Editor
{
    internal static class MovingGameLevelEditorValidator
    {
        public static List<string> ValidateFile(MovingGameLevelsFileData fileData)
        {
            List<string> errors = new();

            if (fileData == null)
            {
                errors.Add("Levels file is null.");
                return errors;
            }

            if (fileData.levels == null)
            {
                errors.Add("Levels list is missing.");
                return errors;
            }

            if (fileData.rankingMetadata == null)
            {
                errors.Add("Ranking metadata is missing.");
                return errors;
            }

            if (fileData.rankingMetadata.maxStars <= 0)
            {
                errors.Add("Ranking metadata maxStars must be greater than 0.");
            }

            if (fileData.rankingMetadata.baseTimeConstantSeconds < 0f)
            {
                errors.Add("Ranking metadata base time constant must be 0 or greater.");
            }

            if (fileData.rankingMetadata.thinkTimePerDifficultySeconds < 0f)
            {
                errors.Add("Ranking metadata think time per difficulty must be 0 or greater.");
            }

            if (fileData.rankingMetadata.instructionTimePerStepSeconds <= 0f)
            {
                errors.Add("Ranking metadata instruction time per step must be greater than 0.");
            }

            if (fileData.rankingMetadata.penaltyWindowBaseSeconds < 0f)
            {
                errors.Add("Ranking metadata penalty window base must be 0 or greater.");
            }

            if (fileData.rankingMetadata.penaltyWindowPerDifficultySeconds < 0f)
            {
                errors.Add("Ranking metadata penalty window per difficulty must be 0 or greater.");
            }

            HashSet<int> ids = new();

            for (int i = 0; i < fileData.levels.Count; i++)
            {
                MovingGameLevelData level = fileData.levels[i];

                if (level == null)
                {
                    errors.Add($"Level at index {i} is null.");
                    continue;
                }

                if (!ids.Add(level.id))
                {
                    errors.Add($"Duplicate level id '{level.id}'.");
                }

                errors.AddRange(ValidateLevel(level).Select(error => $"Level {level.id}: {error}"));
            }

            return errors;
        }

        public static List<string> ValidateLevel(MovingGameLevelData level)
        {
            List<string> errors = new();

            if (level == null)
            {
                errors.Add("Level is null.");
                return errors;
            }

            if (level.id <= 0)
            {
                errors.Add("Id must be greater than 0.");
            }

            if (level.width <= 0 || level.width > 5 || level.height <= 0 || level.height > 5)
            {
                errors.Add("Width and height must be between 1 and 5.");
            }

            if (level.targetInstructionCount <= 0)
            {
                errors.Add("Target instruction count must be greater than 0.");
            }

            if (level.startPosition == null)
            {
                errors.Add("Start position is missing.");
                return errors;
            }

            EnsureCollections(level);

            HashSet<string> obstacleGroups = new();
            HashSet<string> occupied = new();

            ValidatePosition(level, level.startPosition, "startPosition", errors);

            foreach (PositionData food in level.foodPositions)
            {
                ValidatePosition(level, food, "foodPositions", errors);
                AddUnique(occupied, Key(food), "Food", errors);

                if (SamePosition(level.startPosition, food))
                {
                    errors.Add("Food cannot be placed on the start position.");
                }
            }

            foreach (PositionData blocked in level.blockedPositions)
            {
                ValidatePosition(level, blocked, "blockedPositions", errors);
                AddUnique(occupied, Key(blocked), "Blocked cell", errors);

                if (SamePosition(level.startPosition, blocked))
                {
                    errors.Add("Blocked cell cannot be placed on the start position.");
                }
            }

            foreach (PositionData breakable in level.breakableBlockedPositions)
            {
                ValidatePosition(level, breakable, "breakableBlockedPositions", errors);
                AddUnique(occupied, Key(breakable), "Breakable cell", errors);

                if (SamePosition(level.startPosition, breakable))
                {
                    errors.Add("Breakable cell cannot be placed on the start position.");
                }
            }

            foreach (PositionData hole in level.holePositions)
            {
                ValidatePosition(level, hole, "holePositions", errors);
                AddUnique(occupied, Key(hole), "Hole cell", errors);

                if (SamePosition(level.startPosition, hole))
                {
                    errors.Add("Hole cell cannot be placed on the start position.");
                }
            }

            foreach (ToggleBlockedTileData toggleObstacle in level.toggleBlockedTiles)
            {
                ValidatePosition(level, toggleObstacle.x, toggleObstacle.y, "toggleBlockedTiles", errors);

                if (toggleObstacle.groupId <= 0)
                {
                    errors.Add("Toggle obstacle groupId must be greater than 0.");
                }

                AddUnique(occupied, Key(toggleObstacle.x, toggleObstacle.y), "Toggle obstacle", errors);
                obstacleGroups.Add(toggleObstacle.groupId.ToString());

                if (SamePosition(level.startPosition, toggleObstacle.x, toggleObstacle.y))
                {
                    errors.Add("Toggle obstacle cannot be placed on the start position.");
                }
            }

            HashSet<string> switchPositions = new();

            foreach (ToggleSwitchTileData toggleSwitch in level.toggleSwitchTiles)
            {
                ValidatePosition(level, toggleSwitch.x, toggleSwitch.y, "toggleSwitchTiles", errors);

                if (toggleSwitch.groupId <= 0)
                {
                    errors.Add("Toggle switch groupId must be greater than 0.");
                }

                string key = Key(toggleSwitch.x, toggleSwitch.y);
                if (!switchPositions.Add(key))
                {
                    errors.Add("Duplicate toggle switch position.");
                }

                if (occupied.Contains(key))
                {
                    errors.Add("Toggle switch cannot share a cell with food or an obstacle.");
                }

                if (!obstacleGroups.Contains(toggleSwitch.groupId.ToString()))
                {
                    errors.Add($"Toggle switch group '{toggleSwitch.groupId}' has no matching toggle obstacle.");
                }

                if (SamePosition(level.startPosition, toggleSwitch.x, toggleSwitch.y))
                {
                    errors.Add("Toggle switch cannot be placed on the start position.");
                }
            }

            if (level.foodPositions.Count == 0)
            {
                errors.Add("Level must contain at least one food goal.");
            }

            return errors;
        }

        public static void EnsureCollections(MovingGameLevelData level)
        {
            level.foodPositions ??= new List<PositionData>();
            level.blockedPositions ??= new List<PositionData>();
            level.breakableBlockedPositions ??= new List<PositionData>();
            level.holePositions ??= new List<PositionData>();
            level.toggleBlockedTiles ??= new List<ToggleBlockedTileData>();
            level.toggleSwitchTiles ??= new List<ToggleSwitchTileData>();
            level.startPosition ??= new PositionData();
            level.name ??= string.Empty;
            level.hint ??= string.Empty;
            level.startDirection ??= "Right";
            level.targetInstructionCount = System.Math.Max(1, level.targetInstructionCount);
        }

        private static void AddUnique(HashSet<string> occupied, string key, string label, List<string> errors)
        {
            if (!occupied.Add(key))
            {
                errors.Add($"{label} overlaps another occupied cell.");
            }
        }

        private static void ValidatePosition(
            MovingGameLevelData level,
            PositionData position,
            string source,
            List<string> errors)
        {
            if (position == null)
            {
                errors.Add($"{source} contains a null position.");
                return;
            }

            ValidatePosition(level, position.x, position.y, source, errors);
        }

        private static void ValidatePosition(
            MovingGameLevelData level,
            int x,
            int y,
            string source,
            List<string> errors)
        {
            if (x < 0 || x >= level.width || y < 0 || y >= level.height)
            {
                errors.Add($"{source} contains out-of-bounds cell ({x}, {y}).");
            }
        }

        private static bool SamePosition(PositionData position, PositionData other)
        {
            return other != null && SamePosition(position, other.x, other.y);
        }

        private static bool SamePosition(PositionData position, int x, int y)
        {
            return position != null && position.x == x && position.y == y;
        }

        private static string Key(PositionData position)
        {
            return Key(position.x, position.y);
        }

        private static string Key(int x, int y)
        {
            return $"{x}:{y}";
        }
    }
}
