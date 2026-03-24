using System;
using UnityEngine;
using UnityEngine.UI;
using Flowbit.Utilities.Navigation;
using Flowbit.GameBase.Scenes;

namespace Flowbit.MovingGame.Unity
{
    /// <summary>
    /// Popup shown for selecting a level.
    /// </summary>
    public sealed class MovingGameLevelSelectionPopupView : ScreenBase
    {
        [Header("UI")]
        [SerializeField] private Text titleText_;
        [SerializeField] private Text indexText_;
        [SerializeField] private Text hintText_;
        [SerializeField] private Button continueButton_;
        [SerializeField] private Button closeButton_;

        private Action continueAction_;

        /// <summary>
        /// Initializes the popup with navigation parameters.
        /// </summary>
        public override void Initialize(NavigationParams navigationParams)
        {
            if (navigationParams is not MovingGameLevelSelectionPopupParams popupParams)
            {
                throw new ArgumentException(
                    $"Expected {nameof(MovingGameLevelSelectionPopupParams)} but got {navigationParams?.GetType().Name ?? "null"}.");
            }

            continueAction_ = popupParams.OnContinue;

            if (titleText_ != null)
            {
                titleText_.text = popupParams.MovingGameLevelData.name;
            }

            if (hintText_ != null)
            {
                hintText_.text = popupParams.MovingGameLevelData.hint;
            }

            if (indexText_ != null)
            {
                indexText_.text = (popupParams.Index + 1).ToString();
            }

            if (continueButton_ != null)
            {
                continueButton_.onClick.RemoveAllListeners();
                continueButton_.onClick.AddListener(OnContinuePressed);
            }

            if (closeButton_ != null)
            {
                closeButton_.onClick.RemoveAllListeners();
                closeButton_.onClick.AddListener(Close);
            }
        }

        private void OnContinuePressed()
        {
            continueAction_?.Invoke();
            Close();
        }
    }
}