using System.Collections.Generic;
using UnityEngine;
using Flowbit.Utilities.Core.Events;

namespace Flowbit.Utilities.Navigation
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
        /// Gets the installed navigation service.
        /// </summary>
        public INavigationService GetNavigationService()
        {
            return navigationService_;
        }

        /// <summary>
        /// Installs the navigation service.
        /// </summary>
        public void Install(
            EventDispatcher eventDispatcher,
            Dictionary<string, GameObject> prefabs,
            INavigationTransitionStrategy transitionStrategy = null)
        {
            var baseNavigationService = new NavigationService(eventDispatcher);
            navigationService_ = baseNavigationService;

            baseNavigationService.RegisterStrategy(
                NavigationTargetType.Scene,
                new SceneNavigationStrategy(
                    transitionStrategy));

            if (screenNavigationHost_ != null)
            {
                baseNavigationService.RegisterStrategy(
                    NavigationTargetType.Screen,
                    new ScreenNavigationStrategy(
                        prefabs,
                        screenNavigationHost_,
                        transitionStrategy));
            }
        }
    }
}