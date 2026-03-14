namespace CodingGame.Games.Moving
{
    /// <summary>
    /// Represents a position in a 2D grid.
    /// </summary>
    public readonly struct GridPosition
    {
        private readonly int x_;
        private readonly int y_;

        /// <summary>
        /// Creates a grid position.
        /// </summary>
        public GridPosition(int x, int y)
        {
            x_ = x;
            y_ = y;
        }

        /// <summary>
        /// Returns the X coordinate.
        /// </summary>
        public int GetX()
        {
            return x_;
        }

        /// <summary>
        /// Returns the Y coordinate.
        /// </summary>
        public int GetY()
        {
            return y_;
        }

        /// <summary>
        /// Returns a new position offset by the given delta.
        /// </summary>
        public GridPosition Add(int deltaX, int deltaY)
        {
            return new GridPosition(x_ + deltaX, y_ + deltaY);
        }

        /// <summary>
        /// Returns whether this position is equal to another position.
        /// </summary>
        public bool Equals(GridPosition other)
        {
            return x_ == other.x_ && y_ == other.y_;
        }

        /// <summary>
        /// Returns whether this position is equal to another object.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this position.
        /// </summary>
        public override int GetHashCode()
        {
            return System.HashCode.Combine(x_, y_);
        }

        /// <summary>
        /// Returns whether two positions are equal.
        /// </summary>
        public static bool operator ==(GridPosition left, GridPosition right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns whether two positions are different.
        /// </summary>
        public static bool operator !=(GridPosition left, GridPosition right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a debug string for the position.
        /// </summary>
        public override string ToString()
        {
            return $"({x_}, {y_})";
        }
    }
}