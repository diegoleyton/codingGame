using Flowbit.GameBase.Services;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.Scenes;
using Flowbit.Utilities.Navigation;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Builds and handles the game main scene.
    /// </summary>
    public sealed class MainSceneController : SceneBase
    {
        private IGameNavigationService navigationService_;

        protected override void Start()
        {
            base.Start();
            var serviceContainer = GlobalServiceContainer.ServiceContainer;
            navigationService_ = serviceContainer.Get<IGameNavigationService>();
        }

        public void GoToMovingGame()
        {
            navigationService_.Navigate(SceneType.MovingGameLevelSelection);
        }
    }
}