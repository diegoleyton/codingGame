using Flowbit.Utilities.Navigation;
using Flowbit.GameBase.Definitions;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Scene service for this game using transition
    /// </summary>
    public class GameNavigationService : IGameNavigationService
    {
        private INavigationService navigationService_;
        private SceneSettings sceneSettings_;

        public GameNavigationService(
            INavigationService navigationService,
            SceneSettings sceneSettings)
        {
            navigationService_ = navigationService;
            sceneSettings_ = sceneSettings;
        }

        /// <summary>
        /// Navigates to the given type
        /// </summary>
        public void Navigate(
            SceneType sceneType,
            NavigationParams navigationParams = null)
        {
            navigationService_.Navigate(sceneSettings_.GetTarget(sceneType), navigationParams);
        }
    }
}