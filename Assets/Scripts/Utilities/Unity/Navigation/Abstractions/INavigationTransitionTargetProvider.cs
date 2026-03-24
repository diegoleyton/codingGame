using System.Collections.Generic;
using UnityEngine;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Exposes the objects that participate in a navigation transition.
    /// </summary>
    public interface INavigationTransitionTargetProvider
    {
        /// <summary>
        /// Returns the objects that should be animated by the transition strategy.
        /// </summary>
        IReadOnlyList<GameObject> GetTransitionTargets();
    }
}