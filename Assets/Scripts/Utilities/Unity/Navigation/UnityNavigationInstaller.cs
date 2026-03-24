using System.Collections.Generic;
using UnityEngine;
using Flowbit.Utilities.Core.Events;
using Flowbit.Utilities.Core.Navigation;

namespace Flowbit.Utilities.Unity.Navigation
{
    /// <summary>
    /// Installs Unity navigation strategies into navigation services.
    /// </summary>
    public sealed class UnityNavigationInstaller : MonoBehaviour
    {
        [Header("Screen Navigation")]
        [SerializeField] private ScreenNavigationHost screenNavigationHost_;

        private INavigationService navigationService_;

        /// <summary>
        /// Gets the installed core navigation service.
        /// </summary>
        public INavigationService GetNavigationService()
        {
            return navigationService_;
        }

        /// <summary>
        /// Installs the Navigation service.
        /// </summary>
        public void Install(
            EventDispatcher eventDispatcher,
            Dictionary<string, GameObject> prefabs,
            ISceneTransitionStrategy sceneTransitionStrategy = null)
        {
            var baseNavigationService = new NavigationService(eventDispatcher, sceneTransitionStrategy);
            navigationService_ = baseNavigationService;

            baseNavigationService.RegisterStrategy(
                NavigationTargetType.Scene,
                new SceneNavigationStrategy());

            if (screenNavigationHost_ != null)
            {
                baseNavigationService.RegisterStrategy(
                    NavigationTargetType.Screen,
                    new ScreenNavigationStrategy(
                        prefabs,
                        screenNavigationHost_));
            }
        }

        public void InstallTransitionNavigationService(EventDispatcher eventDispatcher, Dictionary<string, GameObject> prefabs)
        {

        }
    }
}