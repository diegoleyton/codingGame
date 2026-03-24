using System.Collections;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Resolves navigation requests using strategies registered per target type.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the given target using the strategy registered for its target type.
        /// </summary>
        IEnumerator Navigate(
            NavigationTarget target,
            NavigationParams navigationParams = null);
    }
}