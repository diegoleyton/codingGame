namespace Flowbit.EngineController
{
    /// <summary>
    /// Represents how well a level was solved, independently of the metric used.
    /// </summary>
    public readonly struct GameRankingResult
    {
        public GameRankingResult(
            int starCount,
            int maxStars,
            string summaryText)
        {
            StarCount = starCount;
            MaxStars = maxStars;
            SummaryText = summaryText ?? string.Empty;
        }

        public int StarCount { get; }
        public int MaxStars { get; }
        public string SummaryText { get; }
        public bool IsValid => MaxStars > 0;
    }
}
