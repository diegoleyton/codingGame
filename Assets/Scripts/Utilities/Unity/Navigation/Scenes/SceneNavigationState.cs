namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Stores scene navigation state between Unity scene loads.
    /// </summary>
    public static class SceneNavigationState
    {
        /// <summary>
        /// Gets or sets the previous navigation target.
        /// </summary>
        public static NavigationTarget FromTarget { get; internal set; }

        /// <summary>
        /// Gets or sets the destination scene target.
        /// </summary>
        public static NavigationTarget ToTarget { get; internal set; }

        /// <summary>
        /// Gets or sets the navigation parameters for the pending scene transition.
        /// </summary>
        public static NavigationParams NavigationParams { get; internal set; }

        /// <summary>
        /// Clears the stored scene navigation state.
        /// </summary>
        public static void Clear()
        {
            FromTarget = null;
            ToTarget = null;
            NavigationParams = null;
        }
    }
}