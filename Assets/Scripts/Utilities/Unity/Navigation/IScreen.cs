using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Represents a screen that can receive navigation parameters when opened.
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// Initializes the screen with the given navigation parameters.
        /// </summary>
        void Initialize(NavigationParams navigationParams);
    }
}