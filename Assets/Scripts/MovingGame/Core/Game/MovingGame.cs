using System;
using System.Collections.Generic;
using System.Linq;

namespace Flowbit.MovingGame.Core
{
    /// <summary>
    /// Represents a simple grid-based game where a character must eat all food items.
    /// </summary>
    public sealed class MovingGame : IMovingGame
    {
        private readonly int width_;
        private readonly int height_;
        private readonly GridPosition startCharacterPosition_;
        private readonly Direction startCharacterDirection_;
        private readonly HashSet<GridPosition> startFoodPositions_;
        private readonly HashSet<GridPosition> blockedPositions_;
        private readonly HashSet<GridPosition> startBreakableBlockedPositions_;
        private readonly Dictionary<GridPosition, ToggleBlockedObstacleState> startToggleBlockedByPosition_;
        private readonly Dictionary<GridPosition, ToggleSwitchTileState> toggleSwitchesByPosition_;

        private GridPosition characterPosition_;
        private Direction characterDirection_;
        private HashSet<GridPosition> foodPositions_;
        private HashSet<GridPosition> breakableBlockedPositions_;
        private Dictionary<GridPosition, ToggleBlockedObstacleState> toggleBlockedByPosition_;
        private HashSet<GridPosition> visitedPositions_;
        private bool hasWon_;
        private bool hasFailed_;
        private StepAfterProcess stepAfterProcess_;

        public MovingGame(
            int width,
            int height,
            GridPosition startCharacterPosition,
            Direction startCharacterDirection,
            IReadOnlyCollection<GridPosition> foodPositions,
            IReadOnlyCollection<GridPosition> blockedPositions = null,
            IReadOnlyCollection<GridPosition> breakableBlockedPositions = null,
            IReadOnlyCollection<ToggleBlockedObstacleState> toggleBlockedObstacles = null,
            IReadOnlyCollection<ToggleSwitchTileState> toggleSwitchTiles = null)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
            }

            if (foodPositions == null)
            {
                throw new ArgumentNullException(nameof(foodPositions));
            }

            width_ = width;
            height_ = height;
            startCharacterPosition_ = startCharacterPosition;
            startCharacterDirection_ = startCharacterDirection;

            ValidatePosition(startCharacterPosition_, nameof(startCharacterPosition));

            startFoodPositions_ = new HashSet<GridPosition>(foodPositions);
            blockedPositions_ = blockedPositions != null
                ? new HashSet<GridPosition>(blockedPositions)
                : new HashSet<GridPosition>();

            startBreakableBlockedPositions_ = breakableBlockedPositions != null
                ? new HashSet<GridPosition>(breakableBlockedPositions)
                : new HashSet<GridPosition>();

            startToggleBlockedByPosition_ = toggleBlockedObstacles != null
                ? toggleBlockedObstacles.ToDictionary(x => x.Position, x => x.Clone())
                : new Dictionary<GridPosition, ToggleBlockedObstacleState>();

            toggleSwitchesByPosition_ = toggleSwitchTiles != null
                ? toggleSwitchTiles.ToDictionary(x => x.Position, x => x)
                : new Dictionary<GridPosition, ToggleSwitchTileState>();

            ValidateFoodPositions();
            ValidateBlockedPositions();
            ValidateToggleTiles();

            foodPositions_ = new HashSet<GridPosition>();
            breakableBlockedPositions_ = new HashSet<GridPosition>();
            toggleBlockedByPosition_ = new Dictionary<GridPosition, ToggleBlockedObstacleState>();
            visitedPositions_ = new HashSet<GridPosition>();

            ResetGame();
        }

        public int GetWidth() => width_;
        public int GetHeight() => height_;
        public GridPosition GetCharacterPosition() => characterPosition_;
        public Direction GetCharacterDirection() => characterDirection_;
        public IReadOnlyCollection<GridPosition> GetFoodPositions() => foodPositions_;
        public IReadOnlyCollection<GridPosition> GetBlockedPositions() => blockedPositions_;
        public IReadOnlyCollection<GridPosition> GetBreakableBlockedPositions() => breakableBlockedPositions_;
        public IReadOnlyCollection<GridPosition> GetVisitedPositions() => visitedPositions_;
        public bool HasStepAfterProcess() => stepAfterProcess_.GetProcessType() != StepAfterProcessType.None;
        public StepAfterProcess GetStepAfterProcess() => stepAfterProcess_;

        public IReadOnlyCollection<ToggleBlockedObstacleState> GetToggleBlockedObstacles()
        {
            return toggleBlockedByPosition_.Values;
        }

        public IReadOnlyCollection<ToggleSwitchTileState> GetToggleSwitchTiles()
        {
            return toggleSwitchesByPosition_.Values;
        }

        public bool TryGetToggleBlockedObstacle(GridPosition position, out ToggleBlockedObstacleState state)
        {
            return toggleBlockedByPosition_.TryGetValue(position, out state);
        }

        public bool TryGetToggleSwitchTile(GridPosition position, out ToggleSwitchTileState state)
        {
            return toggleSwitchesByPosition_.TryGetValue(position, out state);
        }

        public void FinalizeStepAfterProcess()
        {
            if (stepAfterProcess_.GetProcessType() == StepAfterProcessType.Break &&
                stepAfterProcess_.HasPosition() &&
                stepAfterProcess_.IsBreakable())
            {
                breakableBlockedPositions_.Remove(stepAfterProcess_.GetPosition());
            }

            ClearStepAfterProcess();
        }

        public void ClearStepAfterProcess()
        {
            stepAfterProcess_ = StepAfterProcess.None();
        }

        public bool IsBlocked(GridPosition position)
        {
            if (blockedPositions_.Contains(position) || breakableBlockedPositions_.Contains(position))
            {
                return true;
            }

            return toggleBlockedByPosition_.TryGetValue(position, out ToggleBlockedObstacleState state) && state.IsOn;
        }

        public bool IsBreakableBlocked(GridPosition position)
        {
            return breakableBlockedPositions_.Contains(position);
        }

        public bool HasFoodAt(GridPosition position)
        {
            return foodPositions_.Contains(position);
        }

        public bool HasWon() => hasWon_;
        public bool HasFailed() => hasFailed_;

        public void ResetGame()
        {
            characterPosition_ = startCharacterPosition_;
            characterDirection_ = startCharacterDirection_;
            foodPositions_ = new HashSet<GridPosition>(startFoodPositions_);
            breakableBlockedPositions_ = new HashSet<GridPosition>(startBreakableBlockedPositions_);
            toggleBlockedByPosition_ = CloneToggleBlockedDictionary(startToggleBlockedByPosition_);
            visitedPositions_ = new HashSet<GridPosition> { startCharacterPosition_ };
            hasWon_ = false;
            hasFailed_ = false;
            stepAfterProcess_ = StepAfterProcess.None();

            ConsumeFoodAtCurrentPosition();
            ProcessLandingTile(characterPosition_);
            UpdateWinState();
        }

        public void MoveForward(int steps)
        {
            if (steps <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(steps), "Steps must be greater than zero.");
            }

            if (hasWon_ || hasFailed_)
            {
                return;
            }

            ClearStepAfterProcess();
            bool moved = false;

            for (int i = 0; i < steps; i++)
            {
                GridPosition nextPosition = GetForwardPosition(characterPosition_, characterDirection_);

                if (!IsInsideBounds(nextPosition))
                {
                    hasFailed_ = true;
                    return;
                }

                if (IsBlocked(nextPosition))
                {
                    hasFailed_ = true;
                    return;
                }

                characterPosition_ = nextPosition;
                visitedPositions_.Add(characterPosition_);

                ConsumeFoodAtCurrentPosition();
                ProcessLandingTile(characterPosition_);
                UpdateWinState();

                moved = true;

                if (hasWon_)
                {
                    break;
                }
            }

            if (moved)
            {
                stepAfterProcess_ = new StepAfterProcess(
                    StepAfterProcessType.Move,
                    default,
                    false,
                    false);
            }
        }

        public void RotateLeft()
        {
            if (hasWon_ || hasFailed_)
            {
                return;
            }

            ClearStepAfterProcess();

            characterDirection_ = characterDirection_ switch
            {
                Direction.Up => Direction.Left,
                Direction.Left => Direction.Down,
                Direction.Down => Direction.Right,
                Direction.Right => Direction.Up,
                _ => characterDirection_
            };

            stepAfterProcess_ = new StepAfterProcess(
                StepAfterProcessType.Rotate,
                default,
                false,
                false);
        }

        public void RotateRight()
        {
            if (hasWon_ || hasFailed_)
            {
                return;
            }

            ClearStepAfterProcess();

            characterDirection_ = characterDirection_ switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => characterDirection_
            };

            stepAfterProcess_ = new StepAfterProcess(
                StepAfterProcessType.Rotate,
                default,
                false,
                false);
        }

        public void BreakForward()
        {
            if (hasWon_ || hasFailed_)
            {
                return;
            }

            ClearStepAfterProcess();

            GridPosition forwardPosition = GetForwardPosition(characterPosition_, characterDirection_);
            if (!IsInsideBounds(forwardPosition))
            {
                stepAfterProcess_ = new StepAfterProcess(
                    StepAfterProcessType.Break,
                    default,
                    false,
                    false);
                return;
            }

            bool isBreakable = breakableBlockedPositions_.Contains(forwardPosition);
            stepAfterProcess_ = new StepAfterProcess(
                StepAfterProcessType.Break,
                forwardPosition,
                true,
                isBreakable);
        }

        public bool IsInsideBounds(GridPosition position)
        {
            int x = position.GetX();
            int y = position.GetY();
            return x >= 0 && x < width_ && y >= 0 && y < height_;
        }

        private void ProcessLandingTile(GridPosition position)
        {
            if (toggleSwitchesByPosition_.TryGetValue(position, out ToggleSwitchTileState toggleSwitch))
            {
                ToggleGroup(toggleSwitch.GroupId);
            }
        }

        private void ToggleGroup(int groupId)
        {
            foreach (ToggleBlockedObstacleState obstacle in toggleBlockedByPosition_.Values)
            {
                if (obstacle.GroupId == groupId)
                {
                    obstacle.Toggle();
                }
            }
        }

        private Dictionary<GridPosition, ToggleBlockedObstacleState> CloneToggleBlockedDictionary(
            Dictionary<GridPosition, ToggleBlockedObstacleState> source)
        {
            Dictionary<GridPosition, ToggleBlockedObstacleState> clone = new();
            foreach (KeyValuePair<GridPosition, ToggleBlockedObstacleState> pair in source)
            {
                clone[pair.Key] = pair.Value.Clone();
            }

            return clone;
        }

        private void ValidatePosition(GridPosition position, string paramName)
        {
            if (!IsInsideBounds(position))
            {
                throw new ArgumentException("Position must be inside the grid bounds.", paramName);
            }
        }

        private void ValidateFoodPositions()
        {
            foreach (GridPosition foodPosition in startFoodPositions_)
            {
                if (!IsInsideBounds(foodPosition))
                {
                    throw new ArgumentException("Food position must be inside the grid bounds.", nameof(startFoodPositions_));
                }

                if (foodPosition == startCharacterPosition_)
                {
                    throw new ArgumentException("Food position cannot be the start position.", nameof(startFoodPositions_));
                }
            }
        }

        private void ValidateBlockedPositions()
        {
            foreach (GridPosition blockedPosition in blockedPositions_)
            {
                ValidateObstaclePosition(blockedPosition);
            }

            foreach (GridPosition breakableBlockedPosition in startBreakableBlockedPositions_)
            {
                ValidateObstaclePosition(breakableBlockedPosition);

                if (blockedPositions_.Contains(breakableBlockedPosition))
                {
                    throw new ArgumentException(
                        "A position cannot be both a solid and breakable obstacle.",
                        nameof(startBreakableBlockedPositions_));
                }
            }
        }

        private void ValidateToggleTiles()
        {
            foreach (ToggleBlockedObstacleState obstacle in startToggleBlockedByPosition_.Values)
            {
                ValidateObstaclePosition(obstacle.Position);

                if (obstacle.GroupId <= 0)
                {
                    throw new ArgumentException("Toggle obstacle group id must be greater than zero.");
                }

                if (blockedPositions_.Contains(obstacle.Position))
                {
                    throw new ArgumentException("A position cannot be both blocked and toggle-blocked.");
                }

                if (startBreakableBlockedPositions_.Contains(obstacle.Position))
                {
                    throw new ArgumentException("A position cannot be both breakable and toggle-blocked.");
                }
            }

            foreach (ToggleSwitchTileState toggle in toggleSwitchesByPosition_.Values)
            {
                ValidatePosition(toggle.Position, nameof(toggleSwitchesByPosition_));

                if (toggle.GroupId <= 0)
                {
                    throw new ArgumentException("Toggle switch group id must be greater than zero.");
                }

                if (startFoodPositions_.Contains(toggle.Position))
                {
                    throw new ArgumentException("A toggle switch tile cannot also contain food.");
                }
            }
        }

        private void ValidateObstaclePosition(GridPosition obstaclePosition)
        {
            if (!IsInsideBounds(obstaclePosition))
            {
                throw new ArgumentException("Obstacle position must be inside the grid bounds.");
            }

            if (obstaclePosition == startCharacterPosition_)
            {
                throw new ArgumentException("Obstacle position cannot be the start position.");
            }

            if (startFoodPositions_.Contains(obstaclePosition))
            {
                throw new ArgumentException("Obstacle position cannot contain food.");
            }
        }

        private void ConsumeFoodAtCurrentPosition()
        {
            if (foodPositions_.Contains(characterPosition_))
            {
                foodPositions_.Remove(characterPosition_);
            }
        }

        private void UpdateWinState()
        {
            hasWon_ = foodPositions_.Count == 0;
        }

        private GridPosition GetForwardPosition(GridPosition position, Direction direction)
        {
            return direction switch
            {
                Direction.Up => position.Add(0, 1),
                Direction.Right => position.Add(1, 0),
                Direction.Down => position.Add(0, -1),
                Direction.Left => position.Add(-1, 0),
                _ => position
            };
        }
    }
}