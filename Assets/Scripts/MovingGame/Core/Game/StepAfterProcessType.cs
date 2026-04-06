namespace Flowbit.MovingGame.Core
{
    /// <summary>
    /// Represents the type of work that must be completed after a step execution.
    /// </summary>
    public enum StepAfterProcessType
    {
        None = 0,
        Move = 1,
        Rotate = 2,
        Break = 3,
        Jump = 4
    }
}
