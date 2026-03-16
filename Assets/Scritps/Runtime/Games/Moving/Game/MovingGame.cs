using System;

namespace CodingGame.Runtime.Games.Moving
{
    /// <summary>
    /// Represents a simple grid-based game where a character must reach food.
    /// </summary>
    public sealed class MovingGame : IMovingGame
    {
        private readonly int width_;
        private readonly int height_;

        private readonly GridPosition startCharacterPosition_;
        private readonly Direction startCharacterDirection_;
        private readonly GridPosition foodPosition_;

        private GridPosition characterPosition_;
        private Direction characterDirection_;
        private bool hasWon_;
        private bool hasFailed_;

        /// <summary>
        /// Creates a new moving game.
        /// </summary>
        public MovingGame(
            int width,
            int height,
            GridPosition startCharacterPosition,
            Direction startCharacterDirection,
            GridPosition foodPosition)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");
            }

            width_ = width;
            height_ = height;
            startCharacterPosition_ = startCharacterPosition;
            startCharacterDirection_ = startCharacterDirection;
            foodPosition_ = foodPosition;

            ValidatePosition(startCharacterPosition_, nameof(startCharacterPosition));
            ValidatePosition(foodPosition_, nameof(foodPosition));

            ResetGame();
        }

        /// <summary>
        /// Returns the grid width.
        /// </summary>
        public int GetWidth()
        {
            return width_;
        }

        /// <summary>
        /// Returns the grid height.
        /// </summary>
        public int GetHeight()
        {
            return height_;
        }

        /// <summary>
        /// Returns the current character position.
        /// </summary>
        public GridPosition GetCharacterPosition()
        {
            return characterPosition_;
        }

        /// <summary>
        /// Returns the current character direction.
        /// </summary>
        public Direction GetCharacterDirection()
        {
            return characterDirection_;
        }

        /// <summary>
        /// Returns the food position.
        /// </summary>
        public GridPosition GetFoodPosition()
        {
            return foodPosition_;
        }

        /// <summary>
        /// Returns whether the game has been completed successfully.
        /// </summary>
        public bool HasWon()
        {
            return hasWon_;
        }

        /// <summary>
        /// Returns whether the game has reached a failed state.
        /// </summary>
        public bool HasFailed()
        {
            return hasFailed_;
        }

        /// <summary>
        /// Resets the game to its initial state.
        /// </summary>
        public void ResetGame()
        {
            characterPosition_ = startCharacterPosition_;
            characterDirection_ = startCharacterDirection_;
            hasWon_ = false;
            hasFailed_ = false;

            UpdateWinState();
        }

        /// <summary>
        /// Moves the character forward by the given number of steps.
        /// </summary>
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

            for (int i = 0; i < steps; i++)
            {
                GridPosition nextPosition = GetForwardPosition(characterPosition_, characterDirection_);

                if (!IsInsideBounds(nextPosition))
                {
                    hasFailed_ = true;
                    return;
                }

                characterPosition_ = nextPosition;
                UpdateWinState();

                if (hasWon_)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Rotates the character to the left.
        /// </summary>
        public void RotateLeft()
        {
            if (hasWon_ || hasFailed_)
            {
                return;
            }

            characterDirection_ = characterDirection_ switch
            {
                Direction.Up => Direction.Left,
                Direction.Left => Direction.Down,
                Direction.Down => Direction.Right,
                Direction.Right => Direction.Up,
                _ => characterDirection_
            };
        }

        /// <summary>
        /// Rotates the character to the right.
        /// </summary>
        public void RotateRight()
        {
            if (hasWon_ || hasFailed_)
            {
                return;
            }

            characterDirection_ = characterDirection_ switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => characterDirection_
            };
        }

        /// <summary>
        /// Returns whether the given position is inside the grid bounds.
        /// </summary>
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

        private void UpdateWinState()
        {
            hasWon_ = characterPosition_ == foodPosition_;
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