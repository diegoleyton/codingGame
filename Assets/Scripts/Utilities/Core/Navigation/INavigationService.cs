using System;
using System.Collections.Generic;
using Flowbit.Utilities.Core.Events;

namespace Flowbit.Utilities.Core.Navigation
{
    /// <summary>
    /// Resolves navigation requests using strategies registered per target type.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the given target using the strategy registered for its target type.
        /// </summary>
        void Navigate(
            NavigationTarget target,
            NavigationParams navigationParams = null);
    }
}