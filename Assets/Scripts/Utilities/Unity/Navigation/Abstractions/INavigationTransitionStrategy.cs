using System;
using System.Collections;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Plays the visual transition around a navigation operation.
    /// </summary>
    public interface INavigationTransitionStrategy
    {
        /// <summary>
        /// Plays the transition using the provided context and executes the navigation
        /// callback at the appropriate time.
        /// </summary>
        IEnumerator RunTransition(
            NavigationTransitionContext context,
            Action performNavigation);
    }
}