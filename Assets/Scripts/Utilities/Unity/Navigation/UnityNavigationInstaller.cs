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
        [SerializeField] private ScreenEntry[] screenEntries_;

        private NavigationService navigationService_;
        private UnityNavigationService unityNavigationService_;

        /// <summary>
        /// Gets the installed core navigation service.
        /// </summary>
        public NavigationService GetNavigationService()
        {
            return navigationService_;
        }

        /// <summary>
        /// Gets the installed Unity navigation service.
        /// </summary>
        public UnityNavigationService GetUnityNavigationService()
        {
            return unityNavigationService_;
        }

        private void Awake()
        {
            navigationService_ = new NavigationService(GlobalEventDispatcher.EventDispatcher);
            unityNavigationService_ = new UnityNavigationService(navigationService_);

            if (UnityNavigationLocator.Service != null)
            {
                Debug.LogWarning("UnityNavigationLocator.Service is already set. It will be overwritten.");
            }

            UnityNavigationLocator.Service = unityNavigationService_;

            navigationService_.RegisterStrategy(
                NavigationTargetType.Scene,
                new SceneNavigationStrategy());

            if (screenNavigationHost_ != null)
            {
                navigationService_.RegisterStrategy(
                    NavigationTargetType.Screen,
                    new ScreenNavigationStrategy(
                        BuildScreenDictionary(),
                        screenNavigationHost_));
            }
        }

        private Dictionary<string, GameObject> BuildScreenDictionary()
        {
            Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();

            if (screenEntries_ == null)
            {
                return dictionary;
            }

            for (int i = 0; i < screenEntries_.Length; i++)
            {
                ScreenEntry entry = screenEntries_[i];

                if (entry.destination_ == null)
                {
                    continue;
                }

                if (entry.screenPrefab_ == null)
                {
                    continue;
                }

                string destinationId = entry.destination_.GetId();
                dictionary[destinationId] = entry.screenPrefab_;
            }

            return dictionary;
        }

        [Serializable]
        private struct ScreenEntry
        {
            [SerializeField] public NavigationDestinationAsset destination_;
            [SerializeField] public GameObject screenPrefab_;
        }
    }
}