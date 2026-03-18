using System;
using System.Collections.Generic;
using UnityEngine;
using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Navigates to screen prefabs inside the current scene.
    /// </summary>
    public sealed class ScreenNavigationStrategy : INavigationStrategy
    {
        private readonly Dictionary<string, GameObject> prefabsById_;
        private readonly ScreenNavigationHost host_;

        /// <summary>
        /// Creates a new screen navigation strategy.
        /// </summary>
        public ScreenNavigationStrategy(
            Dictionary<string, GameObject> prefabsById,
            ScreenNavigationHost host)
        {
            prefabsById_ = prefabsById ?? throw new ArgumentNullException(nameof(prefabsById));
            host_ = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>
        /// Navigates to the given screen destination.
        /// </summary>
        public void Navigate(NavigationTarget target, NavigationParams navigationParams)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!prefabsById_.TryGetValue(target.Id, out GameObject prefab))
            {
                throw new InvalidOperationException(
                    $"No screen prefab was registered for target id '{target.Id}'.");
            }

            host_.ShowScreen(prefab, navigationParams);
        }
    }
}