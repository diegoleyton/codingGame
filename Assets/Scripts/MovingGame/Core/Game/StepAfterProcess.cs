namespace Flowbit.MovingGame.Core
{
    /// <summary>
    /// Represents additional work that must be completed after a step execution.
    /// </summary>
    public readonly struct StepAfterProcess
    {
        private readonly StepAfterProcessType type_;
        private readonly GridPosition position_;
        private readonly bool hasPosition_;
        private readonly bool isBreakable_;

        /// <summary>
        /// Creates a new step after-process.
        /// </summary>
        public StepAfterProcess(
            StepAfterProcessType type,
            GridPosition position,
            bool hasPosition,
            bool isBreakable)
        {
            type_ = type;
            position_ = position;
            hasPosition_ = hasPosition;
            isBreakable_ = isBreakable;
        }

        /// <summary>
        /// Returns the after-process type.
        /// </summary>
        public StepAfterProcessType GetProcessType()
        {
            return type_;
        }

        /// <summary>
        /// Returns whether the after-process has a target position.
        /// </summary>
        public bool HasPosition()
        {
            return hasPosition_;
        }

        /// <summary>
        /// Returns the target position.
        /// </summary>
        public GridPosition GetPosition()
        {
            return position_;
        }

        /// <summary>
        /// Returns whether the target is breakable.
        /// </summary>
        public bool IsBreakable()
        {
            return isBreakable_;
        }

        /// <summary>
        /// Returns an empty after-process.
        /// </summary>
        public static StepAfterProcess None()
        {
            return new StepAfterProcess(
                StepAfterProcessType.None,
                default,
                false,
                false);
        }
    }
}