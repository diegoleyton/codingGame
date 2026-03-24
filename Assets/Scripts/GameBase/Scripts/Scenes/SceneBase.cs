using UnityEngine;
using Flowbit.GameBase.Services;
using Flowbit.GameBase.Definitions;
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

        public string Id => sceneType_.GetId();

        /// <summary>
        /// Initializes the screen with navigation parameters.
        /// </summary>
        public abstract void Initialize(NavigationParams navigationParams);

        protected IGameNavigationService NavigationService => GlobalServiceContainer.ServiceContainer.Get<IGameNavigationService>();
    }
}