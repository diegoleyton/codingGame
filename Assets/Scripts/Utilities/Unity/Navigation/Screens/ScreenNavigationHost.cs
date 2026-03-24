using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Hosts screen prefabs inside a scene.
    /// </summary>
    public sealed class ScreenNavigationHost : MonoBehaviour
    {
        [SerializeField] private Transform contentRoot_;

        private GameObject currentScreenInstance_;

        /// <summary>
        /// Gets the current instantiated screen, if any.
        /// </summary>
        public GameObject CurrentScreenInstance => currentScreenInstance_;

        /// <summary>
        /// Creates a screen instance under the content root.
        /// </summary>
        public GameObject CreateScreen(GameObject screenPrefab)
        {
            if (screenPrefab == null)
            {
                throw new ArgumentNullException(nameof(screenPrefab));
            }

            if (contentRoot_ == null)
            {
                throw new InvalidOperationException("Content root is not assigned.");
            }

            GameObject instance = Instantiate(screenPrefab, contentRoot_);
            instance.SetActive(false);
            return instance;
        }

        /// <summary>
        /// Initializes the given screen instance.
        /// </summary>
        public void InitializeScreen(GameObject screenInstance, NavigationParams navigationParams)
        {
            if (screenInstance == null)
            {
                throw new ArgumentNullException(nameof(screenInstance));
            }

            IScreen screen = screenInstance.GetComponent<IScreen>();
            if (screen == null)
            {
                return;
            }

            screen.SetCloseAction(HideCurrentScreen);
            screen.Initialize(navigationParams);
        }

        /// <summary>
        /// Replaces the current screen with the provided instance.
        /// </summary>
        public void SetCurrentScreen(GameObject screenInstance)
        {
            if (currentScreenInstance_ != null)
            {
                Destroy(currentScreenInstance_);
            }

            currentScreenInstance_ = screenInstance;

            if (currentScreenInstance_ != null)
            {
                currentScreenInstance_.SetActive(true);
            }
        }

        /// <summary>
        /// Hides and destroys the current screen immediately.
        /// </summary>
        public void HideCurrentScreen()
        {
            if (currentScreenInstance_ == null)
            {
                return;
            }

            Destroy(currentScreenInstance_);
            currentScreenInstance_ = null;
        }

        /// <summary>
        /// Returns the transition targets from the current screen, if available.
        /// </summary>
        public IReadOnlyList<GameObject> GetCurrentTransitionTargets()
        {
            return GetTransitionTargets(currentScreenInstance_);
        }

        /// <summary>
        /// Returns the transition targets from the given screen instance, if available.
        /// </summary>
        public IReadOnlyList<GameObject> GetTransitionTargets(GameObject screenInstance)
        {
            if (screenInstance == null)
            {
                return Array.Empty<GameObject>();
            }

            INavigationTransitionTargetProvider provider =
                screenInstance.GetComponent<INavigationTransitionTargetProvider>();

            return provider?.GetTransitionTargets() ?? Array.Empty<GameObject>();
        }
    }
}