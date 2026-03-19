using Flowbit.Utilities.Core.Navigation;
using Flowbit.GameBase.Definitions;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Scene service for this game
    /// </summary>
    public class GameNavigationService
    {
        private NavigationService navigationService_;
        private SceneSettings sceneSettings_;

        public GameNavigationService(NavigationService navigationService, SceneSettings sceneSettings)
        {
            navigationService_ = navigationService;
            sceneSettings_ = sceneSettings;
        }

        /// <summary>
        /// Navigates to the given type.
        /// </summary>
        public void Navigate(
            SceneType sceneType,
            NavigationParams navigationParams = null)
        {
            navigationService_.Navigate(sceneSettings_.GetTarget(sceneType), navigationParams);
        }
    }
}