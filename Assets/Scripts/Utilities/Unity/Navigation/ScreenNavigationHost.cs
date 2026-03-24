using System;
using System.Collections;
using UnityEngine;
using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Hosts screen prefabs inside a scene and animates transitions between them.
    /// </summary>
    public sealed class ScreenNavigationHost : MonoBehaviour
    {
        [SerializeField]
        private Transform contentRoot_;

        private GameObject currentScreenInstance_;
        private Coroutine transitionCoroutine_;

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

            if (transitionCoroutine_ != null)
            {
                StopCoroutine(transitionCoroutine_);
                transitionCoroutine_ = null;
            }

            transitionCoroutine_ = StartCoroutine(ShowScreenCoroutine(screenPrefab, navigationParams));
        }

        /// <summary>
        /// Hides the current screen if any.
        /// </summary>
        public void HideCurrentScreen()
        {
            if (currentScreenInstance_ == null)
            {
                return;
            }

            IScreen screen = currentScreenInstance_.GetComponent<IScreen>();
            if (screen != null)
            {
                HideScreen(screen);
            }
        }

        /// <summary>
        /// Hides the given screen instance.
        /// </summary>
        public void HideScreen(IScreen screen)
        {
            if (screen == null)
            {
                return;
            }

            MonoBehaviour screenBehaviour = screen as MonoBehaviour;
            if (screenBehaviour == null)
            {
                return;
            }

            GameObject instance = screenBehaviour.gameObject;

            if (instance.transform.parent != contentRoot_)
            {
                return;
            }

            if (transitionCoroutine_ != null)
            {
                StopCoroutine(transitionCoroutine_);
                transitionCoroutine_ = null;
            }

            transitionCoroutine_ = StartCoroutine(HideScreenCoroutine(instance));
        }

        /// <summary>
        /// Clears the current screen immediately.
        /// </summary>
        public void ClearScreen()
        {
            if (transitionCoroutine_ != null)
            {
                StopCoroutine(transitionCoroutine_);
                transitionCoroutine_ = null;
            }

            if (currentScreenInstance_ != null)
            {
                Destroy(currentScreenInstance_);
                currentScreenInstance_ = null;
            }
        }

        private IEnumerator ShowScreenCoroutine(GameObject screenPrefab, NavigationParams navigationParams)
        {
            GameObject previousInstance = currentScreenInstance_;

            IAnimatedScreen previousAnimatedScreen = previousInstance != null
                ? previousInstance.GetComponent<IAnimatedScreen>()
                : null;

            GameObject nextInstance = Instantiate(screenPrefab, contentRoot_);
            nextInstance.SetActive(true);

            IScreen nextScreen = nextInstance.GetComponent<IScreen>();
            nextScreen?.SetCloseAction(HideScreen);
            nextScreen?.Initialize(navigationParams);

            IAnimatedScreen nextAnimatedScreen = nextInstance.GetComponent<IAnimatedScreen>();

            currentScreenInstance_ = nextInstance;

            if (previousAnimatedScreen != null)
            {
                yield return previousAnimatedScreen.PlayExit();
            }

            if (previousInstance != null)
            {
                Destroy(previousInstance);
            }

            if (nextAnimatedScreen != null)
            {
                yield return nextAnimatedScreen.PlayEnter();
            }

            transitionCoroutine_ = null;
        }

        private IEnumerator HideScreenCoroutine(GameObject instance)
        {
            if (currentScreenInstance_ == instance)
            {
                currentScreenInstance_ = null;
            }

            IAnimatedScreen animatedScreen = instance.GetComponent<IAnimatedScreen>();
            if (animatedScreen != null)
            {
                yield return animatedScreen.PlayExit();
            }

            Destroy(instance);
            transitionCoroutine_ = null;
        }
    }
}