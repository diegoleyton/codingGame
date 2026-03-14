namespace CodingGame.Runtime.Core
{
    /// <summary>
    /// Represents the outcome of a single execution step.
    /// </summary>
    public enum StepExecutionStatus
    {
        ExecutedPrimitive,
        CompletedProgram,
        BlockedByGameState
    }
}