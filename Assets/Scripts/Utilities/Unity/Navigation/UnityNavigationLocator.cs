namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Provides global access to the installed Unity navigation service.
    /// </summary>
    public static class UnityNavigationLocator
    {
        /// <summary>
        /// Gets or sets the current Unity navigation service.
        /// </summary>
        public static UnityNavigationService Service { get; internal set; }
    }
}