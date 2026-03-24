using Flowbit.Utilities.Navigation;
using Flowbit.GameBase.Definitions;
using Flowbit.Utilities.Coroutines;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Scene service for this game using transition
    /// </summary>
    public class GameNavigationService : IGameNavigationService
    {
        private INavigationService navigationService_;
        private SceneSettings sceneSettings_;

        private ICoroutineService coroutineService_;

        public GameNavigationService(
            INavigationService navigationService,
            SceneSettings sceneSettings,
            ICoroutineService coroutineService)
        {
            navigationService_ = navigationService;
            sceneSettings_ = sceneSettings;
            coroutineService_ = coroutineService;
        }

        /// <summary>
        /// Navigates to the given type
        /// </summary>
        public void Navigate(
            SceneType sceneType,
            NavigationParams navigationParams = null)
        {
            coroutineService_.StartCoroutine(
                navigationService_.Navigate(
                    sceneSettings_.GetTarget(sceneType),
                    navigationParams));
        }
    }
}