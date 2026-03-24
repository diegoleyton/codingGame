using System;

namespace Flowbit.Utilities.Navigation
{
    /// <summary>
    /// Represents a screen that can receive navigation parameters and request to be closed.
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// Initializes the screen with the given navigation parameters.
        /// </summary>
        void Initialize(NavigationParams navigationParams);

        /// <summary>
        /// Sets the callback used by the screen to request its own close.
        /// </summary>
        void SetCloseAction(Action closeAction);

        /// <summary>
        /// Requests this screen to be closed.
        /// </summary>
        void Close();
    }
}