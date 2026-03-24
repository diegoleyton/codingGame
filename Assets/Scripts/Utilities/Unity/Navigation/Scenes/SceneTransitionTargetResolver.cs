using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Resolves transition target providers from the active Unity scene.
    /// </summary>
    public static class SceneTransitionTargetResolver
    {
        /// <summary>
        /// Returns the first transition target provider found in the active scene.
        /// </summary>
        public static INavigationTransitionTargetProvider ResolveActiveSceneProvider()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (!activeScene.IsValid() || !activeScene.isLoaded)
            {
                return null;
            }

            GameObject[] rootGameObjects = activeScene.GetRootGameObjects();
            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                INavigationTransitionTargetProvider provider =
                    rootGameObjects[i].GetComponentInChildren<INavigationTransitionTargetProvider>(true);

                if (provider != null)
                {
                    return provider;
                }
            }

            return null;
        }
    }
}