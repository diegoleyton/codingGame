using System;
using Flowbit.Utilities.Core.Navigation;
using UnityEngine.SceneManagement;

namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Navigates to Unity scenes.
    /// </summary>
    public sealed class SceneNavigationStrategy : INavigationStrategy
    {
        /// <summary>
        /// Navigates to the given scene destination.
        /// </summary>
        public void Navigate(NavigationTarget target, NavigationParams navigationParams)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            SceneNavigationState.LastSceneTargetId = target.Id;
            SceneNavigationState.LastSceneNavigationParams = navigationParams;

            SceneManager.LoadScene(target.Id);
        }
    }
}