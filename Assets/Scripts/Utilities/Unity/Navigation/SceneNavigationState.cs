using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Stores the last navigation request so the next scene can recover its parameters.
    /// </summary>
    public static class SceneNavigationState
    {
        /// <summary>
        /// Gets or sets the last scene target id.
        /// </summary>
        public static string LastSceneTargetId { get; set; }

        /// <summary>
        /// Gets or sets the last scene navigation parameters.
        /// </summary>
        public static NavigationParams LastSceneNavigationParams { get; set; }

        /// <summary>
        /// Clears the stored scene navigation state.
        /// </summary>
        public static void Clear()
        {
            LastSceneTargetId = null;
            LastSceneNavigationParams = null;
        }
    }
}