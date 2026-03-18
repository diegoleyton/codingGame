using System;
using UnityEngine;
using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Hosts screen prefabs inside a scene.
    /// </summary>
    public sealed class ScreenNavigationHost : MonoBehaviour
    {
        [SerializeField] private Transform contentRoot_;

        private GameObject currentScreenInstance_;

        /// <summary>
        /// Shows the given screen prefab.
        /// </summary>
        public void ShowScreen(GameObject screenPrefab, NavigationParams navigationParams)
        {
            if (screenPrefab == null)
            {
                throw new ArgumentNullException(nameof(screenPrefab));
            }

            if (contentRoot_ == null)
            {
                throw new InvalidOperationException("Content root is not assigned.");
            }

            if (currentScreenInstance_ != null)
            {
                Destroy(currentScreenInstance_);
                currentScreenInstance_ = null;
            }

            currentScreenInstance_ = Instantiate(screenPrefab, contentRoot_);

            IScreen screen = currentScreenInstance_.GetComponent<IScreen>();
            if (screen != null)
            {
                screen.Initialize(navigationParams);
            }
        }

        /// <summary>
        /// Clears the current screen.
        /// </summary>
        public void ClearScreen()
        {
            if (currentScreenInstance_ != null)
            {
                Destroy(currentScreenInstance_);
                currentScreenInstance_ = null;
            }
        }
    }
}