using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Resolves a navigation node from the active scene root hierarchy.
    /// </summary>
    public sealed class SceneNavigationNodeResolver : INavigationNodeResolver
    {
        /// <summary>
        /// Resolves the navigation node for the given target.
        /// </summary>
        public INavigationNode Resolve(NavigationTarget target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Scene activeScene = SceneManager.GetActiveScene();
            GameObject[] roots = activeScene.GetRootGameObjects();

            foreach (GameObject root in roots)
            {
                MonoBehaviour[] behaviours = root.GetComponentsInChildren<MonoBehaviour>(true);

                foreach (MonoBehaviour behaviour in behaviours)
                {
                    if (behaviour is INavigationNode node && node.Id == target.Id)
                    {
                        return node;
                    }
                }
            }

            throw new InvalidOperationException(
                $"No INavigationNode with id '{target.Id}' was found in scene '{activeScene.name}'.");
        }
    }
}