using UnityEngine;
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
        protected void Close()
        {
            NavigationService.Close(sceneType_);
        }
    }
}