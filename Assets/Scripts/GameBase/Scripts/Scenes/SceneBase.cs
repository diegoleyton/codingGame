using UnityEngine;
using System;
using System.Collections;
using Flowbit.GameBase.Services;
using Flowbit.GameBase.Definitions;
using Flowbit.GameBase.UI;
using Flowbit.Utilities.Navigation;
using Flowbit.Utilities.Core.Events;

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

        protected virtual bool IgnoreBackButton => false;

        protected bool initialized_ = false;

        public string Id => sceneType_.GeTInstruction();

        private NavigationParams sceneParameters_;

        private static bool firstScene_ = true;

        protected virtual void Start()
        {
            if (!firstScene_)
            {
                return;
            }

            firstScene_ = false;
            var dispatcher = GlobalServiceContainer.ServiceContainer.Get<EventDispatcher>();
            dispatcher.Send(new OnFirstScene());
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
            yield return null;
            yield return null;
            if (!initialized_)
            {
                InitializeBackButton();
                DefaultInitialize();
            }
        }

        private void InitializeBackButton()
        {
            if (IgnoreBackButton)
            {
                return;
            }
            var backButton = GlobalServiceContainer.ServiceContainer.Get<BackButton>();
            backButton.EnableBackButton(HasBackButton);
        }
    }
}