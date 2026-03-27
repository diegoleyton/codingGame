using UnityEngine;
using System;
using System.Collections;
using Flowbit.GameBase.Services;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.UI;
using Flowbit.Utilities.Navigation;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Base MonoBehaviour implementation for screens.
    /// </summary>
    public abstract class SceneBase : MonoBehaviour, INavigationNode
    {
        [SerializeField]
        protected SceneType sceneType_;

        protected virtual bool HasBackButton => false;

        protected bool initialized_ = false;

        public string Id => sceneType_.GetId();

        private NavigationParams sceneParameters_;

        protected virtual void Start()
        {
            StartCoroutine(InitializeByDefault());
        }

        protected T GetSceneParameters<T>() where T : NavigationParams
        {
            if (sceneParameters_ is not T parameters)
            {
                throw new ArgumentException(
                    $"Expected {nameof(T)} but got {sceneParameters_?.GetType().Name ?? "null"}.");
            }

            return parameters;
        }

        /// <summary>
        /// Initializes the screen with navigation parameters.
        /// </summary>
        public void Initialize(NavigationParams navigationParams)
        {
            sceneParameters_ = navigationParams;
            InitializeBackButton();
            Initialize();
            initialized_ = true;
        }

        protected virtual void Initialize() { }

        protected virtual void DefaultInitialize() { }

        protected IGameNavigationService NavigationService => GlobalServiceContainer.ServiceContainer.Get<IGameNavigationService>();

        private IEnumerator InitializeByDefault()
        {
            yield return new WaitForSeconds(0.5f);
            if (!initialized_)
            {
                InitializeBackButton();
                DefaultInitialize();
            }
        }

        private void InitializeBackButton()
        {
            var backButton = GlobalServiceContainer.ServiceContainer.Get<BackButton>();
            backButton.EnableBackButton(HasBackButton);
        }
    }
}