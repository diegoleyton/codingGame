using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Navigates to screen prefabs inside the current scene.
    /// </summary>
    public sealed class ScreenNavigationStrategy : INavigationStrategy
    {
        private readonly Dictionary<string, GameObject> prefabsById_;
        private readonly ScreenNavigationHost host_;
        private readonly INavigationTransitionStrategy transitionStrategy_;

        private NavigationTarget currentTarget_;

        /// <summary>
        /// Creates a new screen navigation strategy.
        /// </summary>
        public ScreenNavigationStrategy(
            Dictionary<string, GameObject> prefabsById,
            ScreenNavigationHost host,
            INavigationTransitionStrategy transitionStrategy = null)
        {
            prefabsById_ = prefabsById ?? throw new ArgumentNullException(nameof(prefabsById));
            host_ = host ?? throw new ArgumentNullException(nameof(host));
            transitionStrategy_ = transitionStrategy;
        }

        /// <summary>
        /// Navigates to the given screen destination.
        /// </summary>
        public IEnumerator Navigate(NavigationTarget target, NavigationParams navigationParams)
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

            GameObject nextScreen = host_.CreateScreen(prefab);
            host_.InitializeScreen(nextScreen, navigationParams);

            if (transitionStrategy_ == null)
            {
                host_.SetCurrentScreen(nextScreen);
                currentTarget_ = target;
                yield break;
            }

            IReadOnlyList<GameObject> exitTargets = host_.GetCurrentTransitionTargets();
            IReadOnlyList<GameObject> enterTargets = host_.GetTransitionTargets(nextScreen);

            yield return transitionStrategy_.RunTransition(
                new NavigationTransitionContext(
                    currentTarget_,
                    target,
                    navigationParams,
                    exitTargets,
                    enterTargets),
                () => host_.SetCurrentScreen(nextScreen));

            currentTarget_ = target;
        }
    }
}