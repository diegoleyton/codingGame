using System;
using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Provides Unity-friendly navigation helpers on top of the core navigation service.
    /// </summary>
    public sealed class UnityNavigationService
    {
        private readonly NavigationService navigationService_;

        /// <summary>
        /// Creates a new Unity navigation service.
        /// </summary>
        public UnityNavigationService(NavigationService navigationService)
        {
            navigationService_ = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        /// <summary>
        /// Navigates to the given destination asset.
        /// </summary>
        public void Navigate(NavigationDestinationAsset destinationAsset)
        {
            if (destinationAsset == null)
            {
                throw new ArgumentNullException(nameof(destinationAsset));
            }

            navigationService_.Navigate(destinationAsset.CreateTarget());
        }

        /// <summary>
        /// Navigates to the given destination asset using the provided parameters.
        /// </summary>
        public void Navigate(
            NavigationDestinationAsset destinationAsset,
            NavigationParams navigationParams)
        {
            if (destinationAsset == null)
            {
                throw new ArgumentNullException(nameof(destinationAsset));
            }

            navigationService_.Navigate(destinationAsset.CreateTarget(), navigationParams);
        }

        /// <summary>
        /// Returns the wrapped core navigation service.
        /// </summary>
        public NavigationService GetCoreNavigationService()
        {
            return navigationService_;
        }
    }
}