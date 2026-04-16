namespace Flowbit.GameBase.Progress
{
    /// <summary>
    /// Exposes the current level unlock state to the game.
    /// </summary>
    public interface ILevelProgressService
    {
        /// <summary>
        /// Gets the highest unlocked level index.
        /// </summary>
        int GetHighestUnlockedLevelIndex();

        /// <summary>
        /// Returns whether the given level index is currently unlocked.
        /// </summary>
        bool IsLevelUnlocked(int levelIndex);

        /// <summary>
        /// Returns the best amount of stars achieved for the given level index.
        /// </summary>
        int GetBestStarCount(int levelIndex);
    }
}
