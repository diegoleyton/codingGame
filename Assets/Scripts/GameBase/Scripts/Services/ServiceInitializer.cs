using Flowbit.Utilities.Navigation;
using Flowbit.Utilities.Core.Events;
using Flowbit.Utilities.Core.Services;
using Flowbit.Utilities.Coroutines;
using Flowbit.Utilities.Storage;
using Flowbit.Utilities.Unity.UI;
using Flowbit.Utilities.Audio;
using Flowbit.GameBase.Scenes;
using Flowbit.GameBase.Progress;
using Flowbit.GameBase.UI;
using Flowbit.GameBase.Audio;
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
            PreventScreenSleep();
            EventDispatcher dispatcher = new EventDispatcher();
            ServiceContainer.Register(dispatcher);
            InitializeProgressServices(dispatcher);
            var res = InitializeGameResources();
            InitializeScreenBLocker(res);
            CreateCoroutineService();
            InitializeNavigationService(dispatcher, res);
            InitializeUIServices(res);
            InitializeAudioServices(res, dispatcher);
        }

        private GameResources InitializeGameResources()
        {
            var res = Resources.Load<GameResources>("GameResources");
            ServiceContainer.Register(res);
            return res;
        }

        private void InitializeProgressServices(EventDispatcher dispatcher)
        {
            IDataStorage dataStorage = new PlayerPrefsDataStorage();
            ServiceContainer.Register<ILevelProgressService>(new LevelProgressService(dispatcher, dataStorage));
        }

        private void InitializeNavigationService(EventDispatcher dispatcher, GameResources res)
        {
            var sceneTransitionNext = GameObject.Instantiate<GameSceneTransitionBase>(res.SceneSettings.TransitionNext);
            GameObject.DontDestroyOnLoad(sceneTransitionNext);
            var sceneTransitionPrev = GameObject.Instantiate<GameSceneTransitionBase>(res.SceneSettings.TransitionPrev);
            GameObject.DontDestroyOnLoad(sceneTransitionPrev);
            var popupTransitionOpen = GameObject.Instantiate<GameSceneTransitionBase>(res.SceneSettings.PopupTransitionOpen);
            GameObject.DontDestroyOnLoad(popupTransitionOpen);
            var popupTransitionClose = GameObject.Instantiate<GameSceneTransitionBase>(res.SceneSettings.PopupTransitionClose);
            GameObject.DontDestroyOnLoad(popupTransitionClose);

            var navigationPopupContainer = new GameObject("[NavigationPopups]");
            GameObject.DontDestroyOnLoad(navigationPopupContainer);

            INavigationService navigationService = new NavigationService(
                res.SceneSettings.GetPrefabs(),
                navigationPopupContainer.transform,
                new SceneNavigationNodeResolver(),
                sceneTransitionNext,
                sceneTransitionPrev,
                popupTransitionOpen,
                popupTransitionClose,
                ServiceContainer.Get<EventDispatcher>());

            IGameNavigationService transitionNavigationService = new GameNavigationService(
                navigationService,
                res.SceneSettings,
                ServiceContainer.Get<ICoroutineService>(),
                dispatcher,
                ServiceContainer.Get<ScreenBlocker>()
            );
            ServiceContainer.Register(transitionNavigationService);
        }

        private void InitializeScreenBLocker(GameResources res)
        {
            var blockerImage = GameObject.Instantiate<ScreenBlockerImage>(res.SceneSettings.ScreenBlockerImage);
            GameObject.DontDestroyOnLoad(blockerImage);
            ServiceContainer.Register(new ScreenBlocker(blockerImage.Image));
        }

        private void InitializeUIServices(GameResources res)
        {
            var globalAnimator = GameObject.Instantiate<ComponentsLoopAnimator>(res.ComponentLoopAnimator);
            GameObject.DontDestroyOnLoad(globalAnimator);
            ServiceContainer.Register(globalAnimator);

            var backButton = GameObject.Instantiate<BackButton>(res.BackButton);
            GameObject.DontDestroyOnLoad(backButton);
            ServiceContainer.Register(backButton);
        }

        private void InitializeAudioServices(GameResources res, EventDispatcher dispatcher)
        {
            var audioPool = GameObject.Instantiate<GameObject>(res.GameSoundLibrary.AudioPool);
            GameObject.DontDestroyOnLoad(audioPool);
            AudioPlayer<SoundId> audioPlayer = new AudioPlayer<SoundId>(
                res.GameSoundLibrary,
                audioPool.GetComponent<IAudioSourcePool>(),
                audioPool.GetComponent<ILoopingAudioSourcePool>(),
                ServiceContainer.Get<ICoroutineService>());

            new AudioReactor(dispatcher, audioPlayer, res.GameSoundLibrary.MusicTransitionTime);
        }

        private void CreateCoroutineService()
        {
            var go = new GameObject("[CoroutineService]");
            GameObject.DontDestroyOnLoad(go);
            ServiceContainer.Register<ICoroutineService>(go.AddComponent<CoroutineService>());
        }

        private static void PreventScreenSleep()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
