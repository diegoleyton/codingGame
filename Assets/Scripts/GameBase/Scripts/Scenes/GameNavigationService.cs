using Flowbit.Utilities.Navigation;
using Flowbit.GameBase.Definitions;
using Flowbit.Utilities.Coroutines;
using UnityEngine.SceneManagement;

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


            string sceneName = SceneManager.GetActiveScene().name;
            navigationService_.StartWith(sceneSettings_.GetTarget(sceneName));
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

        /// <summary>
        /// Navigates to the previous node
        /// </summary>
        public void Back()
        {
            coroutineService_.StartCoroutine(navigationService_.Back());
        }

        /// <summary>
        /// Closes the opened prefab with the given id.
        /// </summary>
        public void Close(SceneType sceneType)
        {
            string id = sceneSettings_.GetTarget(sceneType).Id;
            coroutineService_.StartCoroutine(navigationService_.Close(id));
        }

        /// <summary>
        /// Navigates to the previous node
        /// </summary>
        public bool CanGoBack => navigationService_.CanGoBack;
    }
}