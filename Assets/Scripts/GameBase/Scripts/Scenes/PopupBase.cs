using UnityEngine;
using UnityEngine.UI;
using Flowbit.GameBase.Services;
using Flowbit.GameBase.Definitions;
using Flowbit.Utilities.Navigation;

namespace Flowbit.GameBase.Scenes
{
    /// <summary>
    /// Base MonoBehaviour implementation for popups.
    /// </summary>
    public abstract class PopupBase : SceneBase
    {
        [field: SerializeField]
        public RectTransform Root { get; private set; }

        [field: SerializeField]
        public Canvas Canvas { get; private set; }

        protected sealed override bool IgnoreBackButton => true;

        protected void Close()
        {
            NavigationService.Close(sceneType_);
        }
    }
}