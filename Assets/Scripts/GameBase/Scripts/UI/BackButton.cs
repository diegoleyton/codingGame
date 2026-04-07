using Flowbit.GameBase.Services;
using Flowbit.GameBase.Scenes;
using UnityEngine;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Defines the back button logic.
    /// </summary>
    public sealed class BackButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject root_;

        private IGameNavigationService navigationService_;

        private void Start()
        {
            var serviceContainer = GlobalServiceContainer.ServiceContainer;
            navigationService_ = serviceContainer.Get<IGameNavigationService>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBack();
            }
        }

        public void GoBack()
        {
            if (root_.activeInHierarchy && root_.activeSelf)
            {
                navigationService_.Back();
            }
        }

        public void EnableBackButton(bool enabled)
        {
            root_.SetActive(enabled);
        }
    }
}
