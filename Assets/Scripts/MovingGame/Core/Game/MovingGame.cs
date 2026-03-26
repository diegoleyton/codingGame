using System;
using System.Collections.Generic;

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

        private GridPosition characterPosition_;
        private Direction characterDirection_;
        private HashSet<GridPosition> foodPositions_;
        private HashSet<GridPosition> breakableBlockedPositions_;
        private HashSet<GridPosition> visitedPositions_;
        private bool hasWon_;
        private bool hasFailed_;
        private StepAfterProcess stepAfterProcess_;

        /// <summary>
        /// Creates a new moving game.
        /// </summary>
        public MovingGame(
            int width,
            int height,
            GridPosition startCharacterPosition,
            Direction startCharacterDirection,
            IReadOnlyCollection<GridPosition> foodPositions,
            IReadOnlyCollection<GridPosition> blockedPositions = null,
            IReadOnlyCollection<GridPosition> breakableBlockedPositions = null)
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

            ValidateFoodPositions();
            ValidateBlockedPositions();

            foodPositions_ = new HashSet<GridPosition>();
            breakableBlockedPositions_ = new HashSet<GridPosition>();
            visitedPositions_ = new HashSet<GridPosition>();

            ResetGame();
        }

        public int GetWidth()
        {
            return width_;
        }

        public int GetHeight()
        {
            return height_;
        }

        public GridPosition GetCharacterPosition()
        {
            return characterPosition_;
        }

        public Direction GetCharacterDirection()
        {
            return characterDirection_;
        }

        public IReadOnlyCollection<GridPosition> GetFoodPositions()
        {
            return foodPositions_;
        }

        public IReadOnlyCollection<GridPosition> GetBlockedPositions()
        {
            return blockedPositions_;
        }

        public IReadOnlyCollection<GridPosition> GetBreakableBlockedPositions()
        {
            return breakableBlockedPositions_;
        }

        /// <summary>
        /// Returns the visited positions.
        /// </summary>
        public IReadOnlyCollection<GridPosition> GetVisitedPositions()
        {
            return visitedPositions_;
        }

        /// <summary>
        /// Returns whether there is a step after-process pending.
        /// </summary>
        public bool HasStepAfterProcess()
        {
            return stepAfterProcess_.GetProcessType() != StepAfterProcessType.None;
        }

        /// <summary>
        /// Returns the current step after-process.
        /// </summary>
        public StepAfterProcess GetStepAfterProcess()
        {
            return stepAfterProcess_;
        }

        /// <summary>
        /// Finalizes the current step after-process.
        /// </summary>
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

        /// <summary>
        /// Clears the current step after-process.
        /// </summary>
        public void ClearStepAfterProcess()
        {
            stepAfterProcess_ = StepAfterProcess.None();
        }

        public bool IsBlocked(GridPosition position)
        {
            return blockedPositions_.Contains(position) || breakableBlockedPositions_.Contains(position);
        }

        public bool IsBreakableBlocked(GridPosition position)
        {
            return breakableBlockedPositions_.Contains(position);
        }

        public bool HasFoodAt(GridPosition position)
        {
            return foodPositions_.Contains(position);
        }

        public bool HasWon()
        {
            return hasWon_;
        }

        public bool HasFailed()
        {
            return hasFailed_;
        }

        public void ResetGame()
        {
            characterPosition_ = startCharacterPosition_;
            characterDirection_ = startCharacterDirection_;
            foodPositions_ = new HashSet<GridPosition>(startFoodPositions_);
            breakableBlockedPositions_ = new HashSet<GridPosition>(startBreakableBlockedPositions_);
            visitedPositions_ = new HashSet<GridPosition> { startCharacterPosition_ };
            hasWon_ = false;
            hasFailed_ = false;
            stepAfterProcess_ = StepAfterProcess.None();

            ConsumeFoodAtCurrentPosition();
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

            bool moved_ = false;

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
                UpdateWinState();
                moved_ = true;

                if (hasWon_)
                {
                    break;
                }
            }

            if (moved_)
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