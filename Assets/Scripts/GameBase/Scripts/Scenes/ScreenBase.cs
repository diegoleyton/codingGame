using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Base MonoBehaviour implementation for screens.
    /// </summary>
    public abstract class ScreenBase : MonoBehaviour, IScreen, INavigationTransitionTargetProvider
    {
        [SerializeField] private GameObject root_;

        private Action closeAction_;

        /// <summary>
        /// Initializes the screen with navigation parameters.
        /// </summary>
        public abstract void Initialize(NavigationParams navigationParams);

        /// <summary>
        /// Sets the callback used by the screen to request its own close.
        /// </summary>
        public void SetCloseAction(Action closeAction)
        {
            closeAction_ = closeAction;
        }

        /// <summary>
        /// Requests this screen to be closed.
        /// </summary>
        public void Close()
        {
            closeAction_?.Invoke();
        }

        /// <summary>
        /// Returns the objects that should participate in the navigation transition.
        /// </summary>
        public IReadOnlyList<GameObject> GetTransitionTargets()
        {
            return new List<GameObject>() { root_ };
        }
    }
}