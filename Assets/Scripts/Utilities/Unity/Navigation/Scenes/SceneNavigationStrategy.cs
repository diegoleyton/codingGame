using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Navigates to Unity scenes.
    /// </summary>
    public sealed class SceneNavigationStrategy : INavigationStrategy
    {
        private readonly INavigationTransitionStrategy transitionStrategy_;

        private NavigationTarget currentTarget_;

        /// <summary>
        /// Creates a new scene navigation strategy.
        /// </summary>
        public SceneNavigationStrategy(
            INavigationTransitionStrategy transitionStrategy = null)
        {
            transitionStrategy_ = transitionStrategy;
        }

        /// <summary>
        /// Navigates to the given scene destination.
        /// </summary>
        public IEnumerator Navigate(NavigationTarget target, NavigationParams navigationParams)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            SceneNavigationState.FromTarget = currentTarget_;
            SceneNavigationState.ToTarget = target;
            SceneNavigationState.NavigationParams = navigationParams;

            if (transitionStrategy_ == null)
            {
                SceneManager.LoadScene(target.Id);
                currentTarget_ = target;
                yield break;
            }

            INavigationTransitionTargetProvider provider =
                SceneTransitionTargetResolver.ResolveActiveSceneProvider();

            yield return transitionStrategy_.RunTransition(
                new NavigationTransitionContext(
                    currentTarget_,
                    target,
                    navigationParams,
                    provider?.GetTransitionTargets() ?? Array.Empty<GameObject>(),
                    Array.Empty<GameObject>()),
                () => SceneManager.LoadScene(target.Id));

            currentTarget_ = target;
        }
    }
}