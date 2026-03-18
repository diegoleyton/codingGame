using Flowbit.Utilities.Core.Events;

namespace Flowbit.Utilities.Core.Navigation
{
    /// <summary>
    /// Raised before a navigation request is resolved by a strategy.
    /// </summary>
    public sealed class NavigationStartedEvent : IEvent
    {
        /// <summary>
        /// Creates a new navigation started event.
        /// </summary>
        public NavigationStartedEvent(NavigationTarget target, NavigationParams navigationParams)
        {
            Target = target;
            NavigationParams = navigationParams;
        }

        /// <summary>
        /// Gets the requested navigation target.
        /// </summary>
        public NavigationTarget Target { get; }

        /// <summary>
        /// Gets the navigation parameters.
        /// </summary>
        public NavigationParams NavigationParams { get; }
    }

    /// <summary>
    /// Raised after a navigation request has been handled by a strategy.
    /// </summary>
    public sealed class NavigationCompletedEvent : IEvent
    {
        /// <summary>
        /// Creates a new navigation completed event.
        /// </summary>
        public NavigationCompletedEvent(NavigationTarget target, NavigationParams navigationParams)
        {
            Target = target;
            NavigationParams = navigationParams;
        }

        /// <summary>
        /// Gets the resolved navigation target.
        /// </summary>
        public NavigationTarget Target { get; }

        /// <summary>
        /// Gets the navigation parameters.
        /// </summary>
        public NavigationParams NavigationParams { get; }
    }
}