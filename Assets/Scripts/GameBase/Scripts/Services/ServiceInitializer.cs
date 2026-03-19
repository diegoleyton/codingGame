using Flowbit.Utilities.Unity.Navigation;
using Flowbit.Utilities.Core.Events;
using Flowbit.Utilities.Core.Services;
using Flowbit.GameBase.Scenes;
using UnityEngine;

namespace Flowbit.GameBase.Services
{
    /// <summary>
    /// Initialize the general services for the game
    /// </summary>
    class ServiceInitializer
    {
        /// <summary>
        /// Gets a service container with the initialized services
        /// </summary>
        public ServiceContainer ServiceContainer { get; private set; }


        /// <summary>
        /// Creates the service initializer and initializes the services
        /// </summary>
        public ServiceInitializer()
        {
            ServiceContainer = new ServiceContainer();

            EventDispatcher dispatcher = new EventDispatcher();
            ServiceContainer.Register(dispatcher);
            var res = InitializeGameResources();
            InitializeNavigationService(dispatcher, res);
        }

        private GameResources InitializeGameResources()
        {
            var res = Resources.Load<GameResources>("GameResources");
            ServiceContainer.Register(res);
            return res;
        }

        private void InitializeNavigationService(EventDispatcher dispatcher, GameResources res)
        {
            var installer = GameObject.Instantiate<UnityNavigationInstaller>(res.NavigationInstallerPrefab);
            GameObject.DontDestroyOnLoad(installer);
            installer.Install(dispatcher, res.SceneSettings.GetPrefabs());
            GameNavigationService GameNavigationService = new GameNavigationService(installer.GetNavigationService(), res.SceneSettings);
            ServiceContainer.Register(GameNavigationService);
        }
    }
}