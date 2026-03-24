namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Contains the information required to play a navigation transition.
    /// </summary>
    public sealed class NavigationTransitionContext
    {
        /// <summary>
        /// Creates a new transition context.
        /// </summary>
        public NavigationTransitionContext(
            NavigationTarget fromTarget,
            NavigationTarget toTarget,
            INavigationNode fromNode,
            INavigationNode toNode,
            NavigationParams navigationParams)
        {
            FromTarget = fromTarget;
            ToTarget = toTarget;
            FromNode = fromNode;
            ToNode = toNode;
            NavigationParams = navigationParams;
        }

        /// <summary>
        /// Gets the previous navigation target.
        /// </summary>
        public NavigationTarget FromTarget { get; }

        /// <summary>
        /// Gets the destination navigation target.
        /// </summary>
        public NavigationTarget ToTarget { get; }

        /// <summary>
        /// Gets the previous node instance.
        /// </summary>
        public INavigationNode FromNode { get; }

        /// <summary>
        /// Gets the destination node instance.
        /// </summary>
        public INavigationNode ToNode { get; }

        /// <summary>
        /// Gets the navigation parameters.
        /// </summary>
        public NavigationParams NavigationParams { get; }
    }
}