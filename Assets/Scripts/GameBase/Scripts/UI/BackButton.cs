using Flowbit.GameBase.Services;
using Flowbit.GameBase.Scenes;
using System;
using System.Reflection;
using UnityEngine;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Defines the back button logic.
    /// </summary>
    public sealed class BackButton : MonoBehaviour
    {
        private static PropertyInfo keyboardCurrentProperty_;
        private static PropertyInfo escapeKeyProperty_;
        private static PropertyInfo wasPressedThisFrameProperty_;

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
            if (WasBackPressedThisFrame())
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

        private static bool WasBackPressedThisFrame()
        {
            if (TryGetInputSystemBackPressed(out bool inputSystemPressed))
            {
                return inputSystemPressed;
            }

#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(KeyCode.Escape);
#else
            return false;
#endif
        }

        private static bool TryGetInputSystemBackPressed(out bool wasPressed)
        {
            wasPressed = false;

            Type keyboardType = Type.GetType("UnityEngine.InputSystem.Keyboard, Unity.InputSystem");
            if (keyboardType == null)
            {
                return false;
            }

            keyboardCurrentProperty_ ??= keyboardType.GetProperty("current", BindingFlags.Public | BindingFlags.Static);
            object keyboard = keyboardCurrentProperty_?.GetValue(null);
            if (keyboard == null)
            {
                return true;
            }

            escapeKeyProperty_ ??= keyboardType.GetProperty("escapeKey", BindingFlags.Public | BindingFlags.Instance);
            object escapeKey = escapeKeyProperty_?.GetValue(keyboard);
            if (escapeKey == null)
            {
                return true;
            }

            wasPressedThisFrameProperty_ ??=
                escapeKey.GetType().GetProperty("wasPressedThisFrame", BindingFlags.Public | BindingFlags.Instance);

            object value = wasPressedThisFrameProperty_?.GetValue(escapeKey);
            if (value is bool pressed)
            {
                wasPressed = pressed;
            }

            return true;
        }
    }
}
