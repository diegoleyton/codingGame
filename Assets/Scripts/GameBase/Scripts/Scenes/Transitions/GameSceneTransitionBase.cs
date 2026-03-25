using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Utilities.Navigation;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Game scene transitions base class for navigation
    /// </summary>
    public abstract class GameSceneTransitionBase : MonoBehaviour, INavigationTransitionStrategy
    {
        /// <summary>
        /// Prepares a new transition and interrupts any previous one that may still be running.
        /// </summary>
        public abstract IEnumerator PrepareTransition(NavigationTransitionContext context);

        /// <summary>
        /// Finishes the currently active transition if it has not been interrupted.
        /// </summary>
        public abstract IEnumerator FinishTransition(NavigationTransitionContext context);
    }
}