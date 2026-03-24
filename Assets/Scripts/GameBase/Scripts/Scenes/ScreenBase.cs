using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Base MonoBehaviour implementation for screens.
    /// </summary>
    public abstract class ScreenBase : SceneBase, IScreen
    {
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
    }
}