using System;
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

        private NavigationService navigationService_;

        /// <summary>
        /// Gets the installed core navigation service.
        /// </summary>
        public NavigationService GetNavigationService()
        {
            return navigationService_;
        }

        /// <summary>
        /// Installs the Navigation service.
        /// </summary>
        public void Install(EventDispatcher eventDispatcher, Dictionary<string, GameObject> prefabs)
        {
            navigationService_ = new NavigationService(eventDispatcher);

            navigationService_.RegisterStrategy(
                NavigationTargetType.Scene,
                new SceneNavigationStrategy());

            if (screenNavigationHost_ != null)
            {
                navigationService_.RegisterStrategy(
                    NavigationTargetType.Screen,
                    new ScreenNavigationStrategy(
                        prefabs,
                        screenNavigationHost_));
            }
        }
    }
}