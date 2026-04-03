namespace Flowbit.Engine
{
    /// <summary>
    /// Represents the result of executing one step in the program runner.
    /// </summary>
    public sealed class StepResult<TInstruction>
    {
        private readonly StepExecutionStatus status_;
        private readonly InstructionInstance<TInstruction> executedInstruction_;

        /// <summary>
        /// Creates a new step result.
        /// </summary>
        public StepResult(StepExecutionStatus status, InstructionInstance<TInstruction> executedInstruction)
        {
            status_ = status;
            executedInstruction_ = executedInstruction;
        }

        /// <summary>
        /// Returns the step execution status.
        /// </summary>
        public StepExecutionStatus GetStatus()
        {
            return status_;
        }

        /// <summary>
        /// Returns the instruction instance executed in this step, if any.
        /// </summary>
        public InstructionInstance<TInstruction> GetExecutedInstruction()
        {
            return executedInstruction_;
        }
    }
}