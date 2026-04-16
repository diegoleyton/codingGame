namespace Flowbit.EngineController
{
    /// <summary>
    /// Tracks how well the player is solving the current level attempt.
    /// </summary>
    public interface IGameRankingTracker
    {
        void Restart();
        GameRankingResult GetCurrentResult();
    }
}
