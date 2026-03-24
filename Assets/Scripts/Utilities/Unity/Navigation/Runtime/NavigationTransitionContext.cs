using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Contains the information required to play a navigation transition.
    /// </summary>
    public sealed class NavigationTransitionContext
    {
        /// <summary>
        /// Creates a new transition context.
        /// </summary>
        public NavigationTransitionContext(
            NavigationTarget fromTarget,
            NavigationTarget toTarget,
            NavigationParams navigationParams,
            IReadOnlyList<GameObject> exitTargets,
            IReadOnlyList<GameObject> enterTargets)
        {
            FromTarget = fromTarget;
            ToTarget = toTarget;
            NavigationParams = navigationParams;
            ExitTargets = exitTargets ?? Array.Empty<GameObject>();
            EnterTargets = enterTargets ?? Array.Empty<GameObject>();
        }

        /// <summary>
        /// Gets the previous navigation target.
        /// </summary>
        public NavigationTarget FromTarget { get; }

        /// <summary>
        /// Gets the destination navigation target.
        /// </summary>
        public NavigationTarget ToTarget { get; }

        /// <summary>
        /// Gets the navigation parameters.
        /// </summary>
        public NavigationParams NavigationParams { get; }

        /// <summary>
        /// Gets the objects that should animate out before navigation.
        /// </summary>
        public IReadOnlyList<GameObject> ExitTargets { get; }

        /// <summary>
        /// Gets the objects that should animate in after navigation.
        /// </summary>
        public IReadOnlyList<GameObject> EnterTargets { get; }
    }
}