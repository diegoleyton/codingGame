using System;
using System.Collections;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Plays a visual transition around a navigation operation.
    /// </summary>
    public interface INavigationTransitionStrategy
    {
        /// <summary>
        /// Plays the transition and executes the given callback at the appropriate time.
        /// </summary>
        IEnumerator RunTransition(
            NavigationTransitionContext context,
            Action performNavigation);
    }
}