using System.Collections;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Defines a strategy capable of navigating to a specific kind of target.
    /// </summary>
    public interface INavigationStrategy
    {
        /// <summary>
        /// Navigates to the given target using the provided parameters.
        /// </summary>
        IEnumerator Navigate(
            NavigationTarget target,
            NavigationParams navigationParams);
    }
}