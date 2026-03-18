using System;
using System.Collections.Generic;
using Flowbit.Utilities.Core.Events;

namespace Flowbit.Utilities.Core.Navigation
{
    /// <summary>
    /// Resolves navigation requests using strategies registered per target type.
    /// </summary>
    public sealed class NavigationService
    {
        private readonly Dictionary<NavigationTargetType, INavigationStrategy> strategies_;
        private readonly EventDispatcher eventDispatcher_;

        /// <summary>
        /// Creates a new navigation service.
        /// </summary>
        public NavigationService(EventDispatcher eventDispatcher = null)
        {
            strategies_ = new Dictionary<NavigationTargetType, INavigationStrategy>();
            eventDispatcher_ = eventDispatcher ?? GlobalEventDispatcher.EventDispatcher;
        }

        /// <summary>
        /// Registers a strategy for the given target type.
        /// </summary>
        public void RegisterStrategy(
            NavigationTargetType targetType,
            INavigationStrategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            strategies_[targetType] = strategy;
        }

        /// <summary>
        /// Unregisters the strategy for the given target type.
        /// </summary>
        public void UnregisterStrategy(NavigationTargetType targetType)
        {
            strategies_.Remove(targetType);
        }

        /// <summary>
        /// Returns whether a strategy is registered for the given target type.
        /// </summary>
        public bool HasStrategy(NavigationTargetType targetType)
        {
            return strategies_.ContainsKey(targetType);
        }

        /// <summary>
        /// Navigates to the given target using the strategy registered for its target type.
        /// </summary>
        public void Navigate(
            NavigationTarget target,
            NavigationParams navigationParams = null)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!strategies_.TryGetValue(target.TargetType, out INavigationStrategy strategy))
            {
                throw new InvalidOperationException(
                    $"No navigation strategy was registered for target type '{target.TargetType}'.");
            }

            eventDispatcher_.Send(new NavigationStartedEvent(target, navigationParams));
            strategy.Navigate(target, navigationParams);
            eventDispatcher_.Send(new NavigationCompletedEvent(target, navigationParams));
        }

        /// <summary>
        /// Removes all registered strategies.
        /// </summary>
        public void ClearStrategies()
        {
            strategies_.Clear();
        }
    }
}