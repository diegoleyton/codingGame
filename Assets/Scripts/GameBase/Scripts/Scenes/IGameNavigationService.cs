using Flowbit.Utilities.Navigation;
using Flowbit.GameBase.Definitions;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Scene service for this game
    /// </summary>
    public interface IGameNavigationService
    {
        /// <summary>
        /// Navigates to the given type.
        /// </summary>
        public void Navigate(SceneType sceneType, NavigationParams navigationParams = null);
    }
}